using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering;
using Chroma.Input;
using Commodore.Framework;
using Commodore.GameLogic.Core.IO.Mappings;
using Commodore.GameLogic.Core.IO.Storage;
using Commodore.GameLogic.Executive.CodeEditor.Bindings;
using Commodore.GameLogic.Executive.CodeEditor.Events;
using Cursor = Commodore.GameLogic.Display.Cursor;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public partial class Editor
    {
        public TrueTypeFont Font { get; }
        public Cursor Cursor { get; }

        public ModeLine ModeLine { get; }
        public TextRenderer TextRenderer { get; }
        public Window Window { get; }

        public List<Hotkey> Hotkeys { get; }

        public EditorBuffer Buffer { get; }
        public EditorOptions Options { get; }

        public bool IsVisible { get; set; }

        public event EventHandler<FileSavedEventArgs> FileSaved;

        public Editor(TrueTypeFont font)
        {
            Font = font;

            Buffer = new EditorBuffer();
            Options = new EditorOptions
            {
                HighlightCurrentLine = true,
                TabSize = 2
            };

            Hotkeys = new List<Hotkey>();
            TextRenderer = new TextRenderer(this);
            Window = new Window(this);

            ModeLine = new ModeLine(Font);
            Cursor = new Cursor
            {
                ForceVisible = true
            };

            _ = new FileSystem(this);
            _ = new Modification(this);
            _ = new Navigation(this);
            _ = new StatusControl(this);
        }

        public void Reset()
        {
            Buffer.Reset();
            Window.Reset();
        }

        public void OpenPath(string path)
        {
            File file;

            try
            {
                file = File.Get(path);
            }
            catch
            {
                file = File.Create(path);
            }

            LoadText(file.GetData());
            Buffer.CurrentFileName = file.GetAbsolutePath();

            IsVisible = true;
        }

        public void LoadText(string fileContents = null)
        {
            Reset();
            Buffer.Lines.Clear();

            if (!string.IsNullOrEmpty(fileContents))
            {
                Buffer.Lines.AddRange(
                    fileContents.Split('\n').Select(x => x.Trim(new[] {'\uFEFF', '\u200B', '\r'}))
                                .ToList()
                );
                Buffer.Dirty = false;
            }
            else
            {
                Buffer.Lines.Add(string.Empty);
                Buffer.Dirty = true;
            }
        }

        public void Update(float deltaTime)
        {
            Cursor.ColorMask = EditorColors.EditorCursorColor;
            Cursor.Update(deltaTime);

            ModeLine.Update(deltaTime);
            UpdateCursorPosition();

            TextRenderer.PushLines(Buffer.Lines);

            if (!ModeLine.IsTakingInput && !ModeLine.PauseDynamicUpdates)
                UpdateModeLineStatusText();
        }

        public void Bind(bool control, bool shift, KeyCode keyCode, Action action)
        {
            Hotkeys.Add(new Hotkey(control, shift, keyCode, action));
        }

        public void UpdateModeLineStatusText()
        {
            var currentLineForHumans = (Buffer.CurrentLineNumber + 1).ToString();
            var lineCountForHumans = Buffer.Lines.Count.ToString();

            if (currentLineForHumans.Length > lineCountForHumans.Length)
            {
                lineCountForHumans = lineCountForHumans.PadLeft(currentLineForHumans.Length, '0');
            }
            else if (currentLineForHumans.Length < lineCountForHumans.Length)
            {
                currentLineForHumans = currentLineForHumans.PadLeft(lineCountForHumans.Length, '0');
            }

            ModeLine.StatusText = $"  {(Buffer.Dirty ? '*' : '-')}  " +
                                  $"{Buffer.CurrentFileName ?? ("<unnamed>")}  " +
                                  $"({currentLineForHumans}/{lineCountForHumans})  " +
                                  $"TM: {Options.TypingMode.ToString().ToUpper()}  " +
                                  $"HL: {(TextRenderer.DisableSyntaxHighlighter ? "OFF" : "ON")}";
        }

        private void DrawBackdrop(RenderContext context)
        {
            context.Rectangle(
                ShapeMode.Fill,
                new Vector2(0, 0),
                G.Window.Size.Width,
                G.Window.Size.Height,
                EditorColors.EditorBackground
            );
        }

        private void DrawLineHighlight(RenderContext context)
        {
            context.Rectangle(
                ShapeMode.Fill,
                new Vector2(
                    Window.LeftMargin * TextRenderer.HorizontalGranularity,
                    Cursor.Y
                ),
                Window.TotalColumns * TextRenderer.HorizontalGranularity,
                TextRenderer.VerticalGranularity,
                EditorColors.EditorLineHighlightColor
            );
        }

        public void Draw(RenderContext context)
        {
            DrawBackdrop(context);
            TextRenderer.Draw(context);

            if (Options.HighlightCurrentLine)
                DrawLineHighlight(context);

            Cursor.Draw(context);
            ModeLine.Draw(context);
        }

        public void UpdateCursorPosition()
        {
            Cursor.SetGranularPosition(
                Buffer.CursorX + (int)Window.LeftMargin,
                Buffer.CursorY + (int)Window.TopMargin
            );
        }

        public void KeyPressed(KeyCode keyCode, KeyModifiers keyModifiers)
        {
            if (ModeLine.IsTakingInput)
            {
                ModeLine.KeyPressed(keyCode, keyModifiers);
                return;
            }

            foreach (var hotkey in Hotkeys.OrderByDescending(x => x.Precedence))
            {
                if (hotkey.IsDown())
                {
                    hotkey.Action();
                    return;
                }
            }

            if (keyCode == KeyCode.Backspace)
                Backspace();
            else if (keyCode == KeyCode.Delete)
                Delete();
            else if (keyCode == KeyCode.Return)
                NewLine();

            if (Options.TypingMode != TypingMode.Regular)
            {
                var dict = PetsciiControlCodes.PetsciiSymbolMappings;

                if (Options.TypingMode == TypingMode.PetsciiShifted)
                    dict = PetsciiControlCodes.PetsciiSymbolMappingShifted;

                if (dict.ContainsKey(keyCode))
                {
                    var petscii = dict[keyCode];

                    if (Font.CanRenderGlyph(petscii))
                        PrintableCharacter(petscii);
                }
            }

            ModeLine.PauseDynamicUpdates = false;
        }

        public void OnFileSaved(string contents, string currentFileName)
        {
            FileSaved?.Invoke(this, new FileSavedEventArgs
            {
                Contents = contents,
                FilePath = currentFileName
            });
        }

        public void TextInput(char character)
        {
            if (char.IsControl(character))
                return;

            if (ModeLine.IsTakingInput)
            {
                ModeLine.TextInput(character);
                return;
            }

            if (Options.TypingMode != TypingMode.Regular)
                return;

            if (Font.CanRenderGlyph(character))
                PrintableCharacter(character);
        }

        public void PrintableCharacter(char c)
        {
            if (Buffer.CursorX == 0)
            {
                Buffer.Lines[Buffer.CurrentLineNumber] = c + Buffer.Lines[Buffer.CurrentLineNumber];
                CursorRight();
                // prepend character to current line, advance cursor right
            }
            else if (Buffer.IsEndOfLine)
            {
                Buffer.Lines[Buffer.CurrentLineNumber] = Buffer.Lines[Buffer.CurrentLineNumber] + c;
                CursorRight();
                //append character to current line, advance cursor right
            }
            else
            {
                var preCursor = Buffer.PreCursorSubstring;
                var postCursor = Buffer.PostCursorSubstring;

                preCursor += c;

                Buffer.Lines[Buffer.CurrentLineNumber] = preCursor + postCursor;
                CursorRight();
                // split current line at cursor, insert printable at the end of first split product, join them back, advance cursor right
            }

            Buffer.Dirty = true;
        }

        public bool LineUp()
        {
            if (Buffer.CurrentLineNumber == 0)
                return false;

            Buffer.CurrentLineNumber--;

            if (Buffer.CurrentLineNumber < Window.Top)
                return Window.MoveUp();

            return false;
        }

        public bool LineDown()
        {
            if (Buffer.CurrentLineNumber >= Buffer.Lines.Count - 1)
                return false;

            Buffer.CurrentLineNumber++;

            if (Buffer.CurrentLineNumber >= Window.Bottom)
                return Window.MoveDown();

            return false;
        }

        public Vector2 CalculateLinePosition(int index)
        {
            return new Vector2(
                Window.LeftMargin * TextRenderer.HorizontalGranularity,
                (Window.TopMargin * TextRenderer.VerticalGranularity) + (index * TextRenderer.VerticalGranularity)
            );
        }
    }
}