using System;
using System.Collections.Generic;
using System.Numerics;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering;
using Commodore.Engine;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Display.Data;

namespace Commodore.GameLogic.Display
{
    public class VGA : EngineObject
    {
        public TrueTypeFont Font;

        public readonly Color DefaultForegroundColor = Color.Gray;
        public readonly Color DefaultBackgroundColor = Color.Black;
        public readonly Color DefaultMarginColor = Color.Gray;

        public Cursor Cursor;

        public char[] CharacterBuffer;
        public Color[] ForegroundColorBuffer;
        public Color[] BackgroundColorBuffer;

        public int TotalColumns;
        public int TotalRows;

        public int CursorX
        {
            get
            {
                var value = Kernel.Instance.Memory.Peek32(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.CursorPositionX);

                if (value > TotalColumns)
                {
                    CursorX = TotalColumns - 1;
                    return TotalColumns - 1;
                }
                else if (value < -1) // compensate for later processing
                {
                    CursorX = 0;
                    return 0;
                }

                return value;
            }
            set
            {
                var val = value;

                if (value > TotalColumns)
                    val = TotalColumns - 1;
                else if (value < -1) // compensate for later processing
                    val = 0;

                Kernel.Instance.Memory.Poke(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.CursorPositionX, val);
            }
        }

        public int CursorY
        {
            get
            {
                var value = Kernel.Instance.Memory.Peek32(SystemConstants.SystemMemoryPlane,
                    SystemMemoryAddresses.CursorPositionY);

                if (value > TotalRows)
                {
                    CursorY = TotalRows - 1;
                    return TotalRows - 1;
                }
                else if (value < 0)
                {
                    CursorY = 0;
                    return 0;
                }

                return value;
            }

            set
            {
                var val = value;

                if (value > TotalRows)
                    val = TotalRows - 1;
                else if (value < 0)
                    val = 0;

                Kernel.Instance.Memory.Poke(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.CursorPositionY,
                    val);
            }
        }

        public byte Margin;
        public byte Padding;

        public Color ActiveForegroundColor
        {
            get => new Color((uint)Kernel.Instance.Memory.Peek32(SystemConstants.SystemMemoryPlane,
                SystemMemoryAddresses.CurrentForegroundColor));
            set => Kernel.Instance.Memory.Poke(SystemConstants.SystemMemoryPlane,
                SystemMemoryAddresses.CurrentForegroundColor, (int)value.PackedValue);
        }

        public Color ActiveBackgroundColor
        {
            get => new Color((uint)Kernel.Instance.Memory.Peek32(SystemConstants.SystemMemoryPlane,
                SystemMemoryAddresses.CurrentBackgroundColor));
            set => Kernel.Instance.Memory.Poke(SystemConstants.SystemMemoryPlane,
                SystemMemoryAddresses.CurrentBackgroundColor, (int)value.PackedValue);
        }

        public Color ActiveMarginColor
        {
            get => new Color((uint)Kernel.Instance.Memory.Peek32(SystemConstants.SystemMemoryPlane,
                SystemMemoryAddresses.CurrentMarginColor));
            set => Kernel.Instance.Memory.Poke(SystemConstants.SystemMemoryPlane,
                SystemMemoryAddresses.CurrentMarginColor, (int)value.PackedValue);
        }

        public event EventHandler InitialSetUpComplete;
        public event EventHandler FailsafeTriggered;

        public VGA(TrueTypeFont font)
        {
            Font = font;
            InitializeDisplayBuffer();
        }

        public Vector2 MeasureString(string str) => Font.Measure(str);

        public void PutCharAt(char character, int x, int y) =>
            PutCharAt(character, ActiveForegroundColor, ActiveBackgroundColor, x, y);

        public void PutCharAt(char character, Color foreground, Color background, int x, int y)
        {
            if (y < 0 || y >= TotalRows || x < 0 || x >= TotalColumns)
            {
                DebugLog.Error($"Tried to put char '{(int)character}' at ({x},{y}) which are out of bounds.");
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
                DebugLog.Error(
                    $"Tried to set color FG:'{foreground.PackedValue:X6}' BG:'{background.PackedValue:X6}' at ({x},{y}) which are out of bounds.");
                return;
            }

            ForegroundColorBuffer[y * TotalColumns + x] = foreground;
            BackgroundColorBuffer[y * TotalColumns + x] = background;
        }

        public override void UpdateOnce(float deltaTime)
        {
            ReloadOffsetParametersIfNeeded();

            InitialSetUpComplete?.Invoke(this, EventArgs.Empty);
            base.UpdateOnce(deltaTime);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            ReloadOffsetParametersIfNeeded();

            Cursor.SetGranularPosition(CursorX + Margin + Padding, CursorY + Margin + Padding);
            Cursor.ColorMask = ActiveForegroundColor;

            Cursor.Update(deltaTime);
        }

        public void Draw(RenderContext context)
        {
            DrawBackdrop(context);
            DrawDisplayBuffer(context);
            Cursor.Draw(context);
        }

        public virtual void ClearScreen(bool preserveColors)
        {
            for (var y = 0; y < TotalRows; y++)
            {
                if (preserveColors)
                {
                    for (var x = 0; x < TotalColumns; x++)
                        CharacterBuffer[y * TotalRows + x] = ' ';
                }
                else
                {
                    for (var x = 0; x < TotalColumns; x++)
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
            for (var y = 1; y < TotalRows; y++)
            {
                for (var x = 0; x < TotalColumns; x++)
                {
                    CharacterBuffer[(y - 1) * TotalColumns + x] = CharacterBuffer[y * TotalColumns + x];
                    ForegroundColorBuffer[(y - 1) * TotalColumns + x] = ForegroundColorBuffer[y * TotalColumns + x];
                    BackgroundColorBuffer[(y - 1) * TotalColumns + x] = BackgroundColorBuffer[y * TotalColumns + x];
                }
            }

            for (var x = 0; x < TotalColumns; x++)
            {
                CharacterBuffer[(TotalRows - 1) * TotalColumns + x] = ' ';
                ForegroundColorBuffer[(TotalRows - 1) * TotalColumns + x] = DefaultForegroundColor;
                BackgroundColorBuffer[(TotalRows - 1) * TotalColumns + x] = DefaultBackgroundColor;
            }
        }

        protected virtual void InitializeDisplayBuffer()
        {
            Cursor = new Cursor();
            RecalculateDimensions();
        }

        protected virtual void DrawBackdrop(RenderContext renderContext)
        {
            renderContext.Rectangle(
                ShapeMode.Fill,
                Vector2.Zero,
                G.Window.Properties.Width,
                G.Window.Properties.Height,
                ActiveMarginColor
            );

            renderContext.Rectangle(
                ShapeMode.Fill,
                new Vector2(
                    Margin * Cursor.Granularity,
                    Margin * Cursor.Granularity
                ),
                G.Window.Properties.Width - (Margin * 2 * Cursor.Granularity),
                G.Window.Properties.Height - (Margin * 2 * Cursor.Granularity),
                ActiveBackgroundColor
            );
        }

        protected void RecalculateDimensions()
        {
            try
            {
                CursorX = 0;
                CursorY = 0;

                TotalColumns = G.Window.Properties.Width / Cursor.Granularity - ((Margin + Padding) * 2);
                TotalRows = G.Window.Properties.Height / Cursor.Granularity - ((Margin + Padding) * 2);

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
            Kernel.Instance.Memory.Poke(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.CurrentMarginSize, 1);
            Kernel.Instance.Memory.Poke(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.CurrentPaddingSize, 1);

            Margin = 1;
            Padding = 1;

            TotalColumns = (int)(G.Window.Properties.Width / Cursor.Granularity) - ((Margin + Padding) * 2);
            TotalRows = (int)(G.Window.Properties.Height / Cursor.Granularity) - ((Margin + Padding) * 2);

            RecalculateDimensions();

            FailsafeTriggered?.Invoke(this, EventArgs.Empty);
        }

        private void ReloadOffsetParametersIfNeeded()
        {
            if (Kernel.Instance.Memory.PeekBool(SystemConstants.SystemMemoryPlane,
                SystemMemoryAddresses.UpdateOffsetParametersFlag))
            {
                Kernel.Instance.Memory.Poke(SystemConstants.SystemMemoryPlane,
                    SystemMemoryAddresses.UpdateOffsetParametersFlag, 0);

                Margin = Kernel.Instance.Memory.Peek8(SystemConstants.SystemMemoryPlane,
                    SystemMemoryAddresses.CurrentMarginSize);
                Padding = Kernel.Instance.Memory.Peek8(SystemConstants.SystemMemoryPlane,
                    SystemMemoryAddresses.CurrentPaddingSize);

                RecalculateDimensions();
            }
        }

        private void DrawDisplayBuffer(RenderContext context)
        {
            var dx = (Margin + Padding) * Cursor.Granularity;

            for (var y = 0; y < TotalRows; y++)
            {
                var start = y * TotalColumns;
                var end = start + TotalColumns;

                var str = new string(CharacterBuffer[start..end]);

                context.DrawString(
                    Font,
                    str,
                    new Vector2(dx, ((Margin + Padding) * Cursor.Granularity) + y * Font.Size),
                    (c, i, p, g) => new GlyphTransformData(p) { Color = ForegroundColorBuffer[y * TotalColumns + i] }
                );
            }

            //foreach (var segment in coloredSegments)
            //{
            //    // context.BlendUsing(BlendingMode.Add);
            //    context.Rectangle(
            //        ShapeMode.Fill,
            //        new Vector2(
            //            dx,
            //            (i + Margin + Padding) * Cursor.Granularity
            //        ),
            //        segment.Value.Length * Cursor.Granularity,
            //        16,
            //        segment.Background
            //    );
            //    // context.BlendUsing(BlendingMode.Default);

            //    context.DrawString(
            //        Font,
            //        segment.Value,
            //        new Vector2(dx, (i + Margin + Padding) * Cursor.Granularity),
            //        (c, i, p, g) => new GlyphTransformData(p) {Color = segment.Foreground});

            //    dx += segment.Width;
            //}
        }
    }
}