using System;
using System.Drawing;
using System.Numerics;
using Chroma.Diagnostics.Logging;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering;
using Commodore.Framework;
using Commodore.GameLogic.Core;
using Color = Chroma.Graphics.Color;

namespace Commodore.GameLogic.Display
{
    public class VGA
    {
        private bool _alreadyInitialized;

        private Log Log { get; } = LogManager.GetForCurrentAssembly();

        public TrueTypeFont Font;

        public readonly Color DefaultForegroundColor = Color.Gray;
        public readonly Color DefaultBackgroundColor = Color.Black;

        public Cursor Cursor;

        public char[] CharacterBuffer;
        public Color[] ForegroundColorBuffer;
        public Color[] BackgroundColorBuffer;

        public int TotalColumns;
        public int TotalRows;

        public VgaMargins Margins;

        public int CursorX
        {
            get
            {
                var value = Kernel.Instance.Memory.Peek32(SystemMemoryAddresses.CursorPositionX);

                if (value > TotalColumns - Margins.Right)
                {
                    CursorX = TotalColumns - Margins.Right - 1;
                    return TotalColumns - Margins.Right - 1;
                }
                else if (value < Margins.Left - 1)
                {
                    CursorX = Margins.Left;
                    return Margins.Left;
                }

                return value;
            }
            set
            {
                var val = value;

                if (val > TotalColumns - Margins.Right)
                    val = TotalColumns - Margins.Right - 1;
                else if (val < Margins.Left - 1)
                    val = Margins.Left;

                Kernel.Instance.Memory.Poke(SystemMemoryAddresses.CursorPositionX, val);
            }
        }

        public int CursorY
        {
            get
            {
                var value = Kernel.Instance.Memory.Peek32(SystemMemoryAddresses.CursorPositionY);

                if (value > TotalRows - Margins.Bottom)
                {
                    CursorY = TotalRows - Margins.Bottom;
                    return TotalRows - Margins.Bottom;
                }
                else if (value < Margins.Top)
                {
                    CursorY = Margins.Top;
                    return Margins.Top;
                }

                return value;
            }

            set
            {
                var val = value;

                if (value > TotalRows - Margins.Bottom)
                    val = TotalRows - Margins.Bottom ;
                else if (value < Margins.Top)
                    val = Margins.Top;

                Kernel.Instance.Memory.Poke(SystemMemoryAddresses.CursorPositionY, val);
            }
        }


        public Color ActiveForegroundColor
        {
            get => new Color((uint)Kernel.Instance.Memory.Peek32(SystemMemoryAddresses.CurrentForegroundColor));
            set => Kernel.Instance.Memory.Poke(SystemMemoryAddresses.CurrentForegroundColor, (int)value.PackedValue);
        }

        public Color ActiveBackgroundColor
        {
            get => new Color((uint)Kernel.Instance.Memory.Peek32(SystemMemoryAddresses.CurrentBackgroundColor));
            set => Kernel.Instance.Memory.Poke(SystemMemoryAddresses.CurrentBackgroundColor, (int)value.PackedValue);
        }

        public event EventHandler InitialSetUpComplete;
        public event EventHandler FailsafeTriggered;

        public VGA(TrueTypeFont font)
        {
            Font = font;
            InitializeDisplayBuffer();
        }

        public Size MeasureString(string str) => Font.Measure(str);

        public void PutCharAt(char character, int x, int y) =>
            PutCharAt(character, ActiveForegroundColor, ActiveBackgroundColor, x, y);

        public void PutCharAt(char character, Color foreground, Color background, int x, int y)
        {
            if (y < 0 || y >= TotalRows || x < 0 || x >= TotalColumns)
            {
                Log.Error($"Tried to put char '{(int)character}' at ({x},{y}) which are out of bounds.");
                return;
            }

            CharacterBuffer[y * TotalColumns + x] = character;
            ForegroundColorBuffer[y * TotalColumns + x] = foreground;
            BackgroundColorBuffer[y * TotalColumns + x] = background;
        }

        public void SetColorsAt(Color foreground, Color background, int x, int y)
        {
            if (y < 0 || y >= TotalRows || x < 0 || x >= TotalColumns)
            {
                Log.Error(
                    $"Tried to set color FG:'{foreground.PackedValue:X6}' BG:'{background.PackedValue:X6}' at ({x},{y}) which are out of bounds.");
                return;
            }

            ForegroundColorBuffer[y * TotalColumns + x] = foreground;
            BackgroundColorBuffer[y * TotalColumns + x] = background;
        }

        public void ResetInitialization()
        {
            _alreadyInitialized = false;
        }

        public void Update(float deltaTime)
        {
            ReloadOffsetParametersIfNeeded();

            if (!_alreadyInitialized)
            {
                _alreadyInitialized = true;

                InitialSetUpComplete?.Invoke(this, EventArgs.Empty);

                Kernel.Instance.Memory.Poke(
                    SystemMemoryAddresses.SoftResetCompleteFlag,
                    false
                );
            }

            Cursor.SetGranularPosition(CursorX, CursorY);
            Cursor.ColorMask = ActiveForegroundColor;

            Cursor.Update(deltaTime);
        }

        public void Draw(RenderContext context)
        {
            DrawBackdrop(context);
            DrawDisplayBuffer(context);

            Cursor.Draw(context);
        }

        public void ClearScreen(bool preserveColors)
        {
            for (var y = Margins.Top; y < TotalRows - Margins.Bottom; y++)
            {
                if (preserveColors)
                {
                    for (var x = Margins.Left; x < TotalColumns - Margins.Right; x++)
                        CharacterBuffer[y * TotalRows + x] = ' ';
                }
                else
                {
                    for (var x = Margins.Left; x < TotalColumns - Margins.Right; x++)
                    {
                        CharacterBuffer[y * TotalColumns + x] = ' ';
                        ForegroundColorBuffer[y * TotalColumns + x] = DefaultForegroundColor;
                        BackgroundColorBuffer[y * TotalColumns + x] = DefaultBackgroundColor;
                    }

                    ActiveForegroundColor = DefaultForegroundColor;
                    ActiveBackgroundColor = DefaultBackgroundColor;
                }
            }
        }

        public virtual void ScrollUp()
        {
            for (var y = Margins.Top + 1; y < TotalRows - Margins.Bottom; y++)
            {
                for (var x = Margins.Left; x < TotalColumns - Margins.Right; x++)
                {
                    CharacterBuffer[(y - 1) * TotalColumns + x] = CharacterBuffer[y * TotalColumns + x];
                    ForegroundColorBuffer[(y - 1) * TotalColumns + x] = ForegroundColorBuffer[y * TotalColumns + x];
                    BackgroundColorBuffer[(y - 1) * TotalColumns + x] = BackgroundColorBuffer[y * TotalColumns + x];
                }
            }

            for (var x = Margins.Left; x < TotalColumns - Margins.Right; x++)
            {
                CharacterBuffer[(TotalRows - Margins.Bottom - 1) * TotalColumns + x] = ' ';
                ForegroundColorBuffer[(TotalRows - Margins.Bottom - 1) * TotalColumns + x] = DefaultForegroundColor;
                BackgroundColorBuffer[(TotalRows - Margins.Bottom - 1) * TotalColumns + x] = DefaultBackgroundColor;
            }
        }

        protected void InitializeDisplayBuffer()
        {
            Cursor = new Cursor();
            RecalculateDimensions();
        }

        protected void DrawBackdrop(RenderContext renderContext)
        {
            renderContext.Rectangle(
                ShapeMode.Fill,
                Vector2.Zero,
                G.Window.Size.Width * Cursor.Granularity,
                G.Window.Size.Height * Cursor.Granularity,
                ActiveBackgroundColor
            );
        }

        protected void RecalculateDimensions()
        {
            try
            {
                CursorX = Margins.Left;
                CursorY = Margins.Top;

                TotalColumns = G.Window.Size.Width / Cursor.Granularity;
                TotalRows = G.Window.Size.Height / Cursor.Granularity;

                CharacterBuffer = new char[TotalColumns * TotalRows];
                ForegroundColorBuffer = new Color[TotalColumns * TotalRows];
                BackgroundColorBuffer = new Color[TotalColumns * TotalRows];

                for (var y = 0; y < TotalRows; y++)
                {
                    for (var x = 0; x < TotalColumns; x++)
                    {
                        CharacterBuffer[y * TotalColumns + x] = ' ';
                        ForegroundColorBuffer[y * TotalColumns + x] = Color.White;
                        BackgroundColorBuffer[y * TotalColumns + x] = Color.Black;
                    }
                }
            }
            catch (OverflowException) // for when margin/padding combination is too large
            {
                FailsafeReset();
            }
        }

        private void FailsafeReset()
        {
            Kernel.Instance.Memory.Poke(SystemMemoryAddresses.MarginArea, 0x00000000);

            Margins = new VgaMargins {Left = 0, Top = 0, Right = 0, Bottom = 0};

            TotalColumns = G.Window.Size.Width / Cursor.Granularity;
            TotalRows = G.Window.Size.Height / Cursor.Granularity;

            RecalculateDimensions();

            FailsafeTriggered?.Invoke(this, EventArgs.Empty);
        }

        private void ReloadOffsetParametersIfNeeded()
        {
            if (Kernel.Instance.Memory.PeekBool(SystemMemoryAddresses.UpdateOffsetParametersFlag))
            {
                Kernel.Instance.Memory.Poke(SystemMemoryAddresses.UpdateOffsetParametersFlag, 0);

                Margins.Left = Kernel.Instance.Memory.Peek8(SystemMemoryAddresses.MarginArea + 3);
                Margins.Top = Kernel.Instance.Memory.Peek8(SystemMemoryAddresses.MarginArea + 2);
                Margins.Right = Kernel.Instance.Memory.Peek8(SystemMemoryAddresses.MarginArea + 1);
                Margins.Bottom = Kernel.Instance.Memory.Peek8(SystemMemoryAddresses.MarginArea + 0);

                RecalculateDimensions();
            }
        }

        public void SetMargins(byte left, byte top, byte right, byte bottom)
        {
            Kernel.Instance.Memory.Poke(SystemMemoryAddresses.MarginArea + 3, left);
            Kernel.Instance.Memory.Poke(SystemMemoryAddresses.MarginArea + 2, top);
            Kernel.Instance.Memory.Poke(SystemMemoryAddresses.MarginArea + 1, right);
            Kernel.Instance.Memory.Poke(SystemMemoryAddresses.MarginArea + 0, bottom);

            Kernel.Instance.Memory.Poke(SystemMemoryAddresses.UpdateOffsetParametersFlag, (byte)1);
            ReloadOffsetParametersIfNeeded();
        }

        private void DrawDisplayBuffer(RenderContext context)
        {
            var dx = 0;

            for (var y = 0; y < TotalRows; y++)
            {
                var start = y * TotalColumns;
                var end = start + TotalColumns;

                var str = new string(CharacterBuffer[start..end]);

                try
                {
                    context.DrawString(
                        Font,
                        str,
                        new Vector2(dx, y * Font.Size),
                        (c, i, p, g) => new GlyphTransformData(p) {Color = ForegroundColorBuffer[y * TotalColumns + i]}
                    );
                }
                catch
                {
                    /* Ignored */
                }
            }
        }
    }
}