using Chroma.Input;
using System;

namespace Commodore.GameLogic.Executive.CodeEditor.Bindings
{
    public class Navigation : KeyBindings
    {
        public Navigation(Editor editor) : base(editor)
        {
            editor.Bind(false, false, KeyCode.End, End);
            editor.Bind(false, false, KeyCode.Home, Home);

            editor.Bind(true, false, KeyCode.A, FileStart);
            editor.Bind(true, false, KeyCode.Z, FileEnd);

            editor.Bind(true, false, KeyCode.G, GoToLine);

            editor.Bind(false, false, KeyCode.PageUp, PageUp);
            editor.Bind(false, false, KeyCode.PageDown, PageDown);

            editor.Bind(false, false, KeyCode.Left, Left);
            editor.Bind(false, false, KeyCode.Up, Up);
            editor.Bind(false, false, KeyCode.Right, Right);
            editor.Bind(false, false, KeyCode.Down, Down);

            editor.Bind(true, false, KeyCode.Left, PreviousWord);
            editor.Bind(true, false, KeyCode.Right, NextWord);
        }

        public void PreviousWord()
        {
            Editor.CursorLeft();

            while (Editor.Buffer.CursorX > 0 && char.IsLetterOrDigit(Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber][Editor.Buffer.CursorX - 1]))
                Editor.CursorLeft();
        }

        public void NextWord()
        {
            Editor.CursorRight();

            while (Editor.Buffer.CursorX < Editor.Buffer.CurrentLine.Length && char.IsLetterOrDigit(Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber][Editor.Buffer.CursorX]))
                Editor.CursorRight();
        }

        public void Left()
        {
            Editor.CursorLeft();
        }

        public void Up()
        {
            Editor.CursorUp();
        }

        public void Right()
        {
            Editor.CursorRight();
        }

        public void Down()
        {
            Editor.CursorDown();
        }

        public void FileEnd()
        {
            while (Editor.Buffer.CurrentLineNumber + 1 < Editor.Buffer.Lines.Count)
                Editor.CursorDown();
        }

        public void FileStart()
        {
            while (Editor.Buffer.CurrentLineNumber > 0)
                Editor.CursorUp();
        }

        public void GoToLine()
        {
            void inputReceived(object sender, string input)
            {
                Editor.ModeLine.InputReceived -= inputReceived;
                Editor.ModeLine.InputCanceled -= inputCanceled;

                var success = int.TryParse(input, out int result);

                if (!success)
                {
                    Editor.ModeLine.Notify("INVALID LINE NUMBER");
                    return;
                }
                else
                {
                    var actualIndex = result - 1;

                    if (actualIndex >= Editor.Buffer.Lines.Count || actualIndex < 0)
                    {
                        Editor.ModeLine.Notify("LINE NUMBER OUT OF RANGE");
                        return;
                    }

                    GoToLine(actualIndex);
                }
            }

            void inputCanceled(object sender, EventArgs e)
            {
                Editor.ModeLine.InputCanceled -= inputCanceled;
                Editor.ModeLine.InputReceived -= inputReceived;
            }

            Editor.ModeLine.InputCanceled += inputCanceled;
            Editor.ModeLine.InputReceived += inputReceived;

            Editor.ModeLine.Read("LINE NUMBER? ");
        }

        public void End()
        {
            Editor.Buffer.CursorX = Editor.Buffer.CurrentLineLength;
        }

        public void Home()
        {
            Editor.Buffer.CursorX = 0;
        }

        public void PageUp()
        {
            while (true)
            {
                if (Editor.Buffer.PreviousLine == null)
                {
                    break;
                }

                Editor.CursorUp();
            }
        }

        public void PageDown()
        {
            while (true)
            {
                if (Editor.Buffer.NextLine == null)
                {
                    break;
                }

                Editor.CursorDown();
            }
        }

        private void GoToLine(int index)
        {
            if (index < 0 || index >= Editor.Buffer.Lines.Count)
                return;

            if (index > Editor.Buffer.CurrentLineNumber)
            {
                while (Editor.Buffer.CurrentLineNumber != index)
                    Editor.CursorDown();
            }
            else if (index < Editor.Buffer.CurrentLineNumber)
            {
                while (Editor.Buffer.CurrentLineNumber != index)
                    Editor.CursorUp();
            }
        }
    }
}
