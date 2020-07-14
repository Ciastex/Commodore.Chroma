using Commodore.Engine;
using Commodore.GameLogic.Display;
using Chroma;
using Chroma.Graphics;
using Chroma.Input;
using System;
using System.Numerics;
using Chroma.Graphics.TextRendering;
using Cursor = Commodore.GameLogic.Display.Cursor;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public class ModeLine
    {
        private readonly TrueTypeFont _font;

        public bool PauseDynamicUpdates { get; set; }
        public bool IsTakingInput { get; private set; }

        private uint ScreenWidth { get; set; }
        private uint ScreenHeight { get; set; }

        private Cursor InputCursor { get; set; }

        public int ActualHeight { get; }
        public int GranularHeight { get; }
        public int Granularity { get; }

        public string InputText { get; private set; }
        public string StatusText { get; set; }

        public event EventHandler InputCanceled;
        public event EventHandler<string> InputReceived;

        public ModeLine(TrueTypeFont font)
        {
            _font = font;

            GranularHeight = 1;
            Granularity = _font.Size;

            ActualHeight = GranularHeight * Granularity;
            StatusText = string.Empty;

            InputCursor = new Cursor
            {
                ColorMask = EditorColors.ModeLineForeground,
                ForceVisible = true
            };

            InputText = string.Empty;
        }

        public void Update(float deltaTime)
        {
            ScreenWidth = (uint)G.Window.Size.Width;
            ScreenHeight = (uint)G.Window.Size.Height;

            if (IsTakingInput)
            {
                var promptTextWidth = StatusText.Length * Granularity;
                var inputTextWidth = InputText.Length * Granularity;

                InputCursor.SetPixelBasedPosition(
                    promptTextWidth + inputTextWidth,
                    (int)(ScreenHeight - ActualHeight)
                );
            }

            InputCursor.Update(deltaTime);
        }

        public void Draw(RenderContext context)
        {
            var targetTextPosition = new Vector2(0, ScreenHeight - ActualHeight);

            context.Rectangle(
                ShapeMode.Fill,
                new Vector2( 
                    0,
                    ScreenHeight - ActualHeight - 2
                ),
                ScreenWidth,
                ActualHeight + 2,
                EditorColors.ModeLineBackground
            );

            var str = StatusText;
            if (IsTakingInput)
                str = StatusText + InputText;

            context.DrawString(
                _font,
                str,
                targetTextPosition,
                (c, i, p, g) => new GlyphTransformData(p)
                {
                    Color = EditorColors.ModeLineForeground
                }
            );


            if (IsTakingInput)
                InputCursor.Draw(context);
        }

        public void Notify(string message)
        {
            PauseDynamicUpdates = true;
            StatusText = message;
        }

        public void Read(string prompt)
        {
            StatusText = prompt;
            IsTakingInput = true;
        }

        public void KeyPressed(KeyCode keyCode, KeyModifiers keyModifiers)
        {
            if (keyCode == KeyCode.Return)
            {
                InputReceived?.Invoke(this, InputText);
                InputText = string.Empty;
                IsTakingInput = false;
            }
            else if (keyCode == KeyCode.Backspace)
            {
                if (InputText.Length == 0)
                    return;

                InputText = InputText.Substring(0, InputText.Length - 1);
            }
            else if (keyCode == KeyCode.Escape)
            {
                InputText = string.Empty;
                IsTakingInput = false;

                InputCanceled?.Invoke(this, EventArgs.Empty);
            }
        }

        public void TextInput(char character)
        {
            if (!IsTakingInput)
                return;

            if (!_font.CanRenderGlyph(character))
                return;

            InputText += character;
        }
    }
}