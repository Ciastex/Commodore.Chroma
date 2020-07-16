using Chroma.Input;

namespace Commodore.GameLogic.Executive.CodeEditor.Bindings
{
    public class Modification : KeyBindings
    {
        public Modification(Editor editor) : base(editor)
        {
            editor.Bind(true, false, KeyCode.Up, MoveLineUp);
            editor.Bind(true, false, KeyCode.Down, MoveLineDown);

            editor.Bind(true, false, KeyCode.D, Duplicate);
            editor.Bind(false, false, KeyCode.Tab, Tab);
            editor.Bind(true, false, KeyCode.K, Kill);
            editor.Bind(true, false, KeyCode.Y, Yank);

            editor.Bind(true, false, KeyCode.Backspace, BackspaceBlock);
        }

        public void Duplicate()
        {
            Editor.Buffer.Lines.Insert(Editor.Buffer.CurrentLineNumber, Editor.Buffer.CurrentLine);
            Editor.Buffer.Dirty = true;
        }

        public void MoveLineUp()
        {
            if (Editor.Buffer.CurrentLineNumber - 1 < 0)
                return;

            var line = Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber - 1];
            Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber - 1] = Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber];
            Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber] = line;

            Editor.CursorUp();
            Editor.Buffer.Dirty = true;
        }

        public void MoveLineDown()
        {
            if (Editor.Buffer.CurrentLineNumber + 1 >= Editor.Buffer.Lines.Count)
                return;

            var line = Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber + 1];
            Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber + 1] = Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber];
            Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber] = line;

            Editor.CursorDown();
            Editor.Buffer.Dirty = true;
        }

        public void BackspaceBlock()
        {
            var runAgain = false;

            if (Editor.Buffer.CursorX != 0 && char.IsLetter(Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber][Editor.Buffer.CursorX - 1]))
                runAgain = true;

            Editor.Backspace();

            if (runAgain)
                BackspaceBlock();
        }

        public void Tab()
        {
            for (var i = 0; i < Editor.Options.TabSize; i++)
                Editor.PrintableCharacter(' ');
        }

        public void Kill()
        {
            Editor.Buffer.LastKill = new string(Editor.Buffer.Lines[Editor.Buffer.CurrentLineNumber]);
            Editor.Buffer.Lines.RemoveAt(Editor.Buffer.CurrentLineNumber);

            if (Editor.Buffer.Lines.Count == 0)
                Editor.Buffer.Lines.Add(string.Empty);

            if (Editor.Buffer.CurrentLineNumber >= Editor.Buffer.Lines.Count)
                Editor.CursorUp();

            if (Editor.Buffer.CursorX >= Editor.Buffer.CurrentLineLength)
                Editor.Buffer.CursorX = Editor.Buffer.CurrentLineLength;

            Editor.Buffer.Dirty = true;
        }

        public void Yank()
        {
            if (Editor.Buffer.LastKill == null)
                return;

            Editor.Buffer.Lines.Insert(Editor.Buffer.CurrentLineNumber, Editor.Buffer.LastKill);
            Editor.Buffer.Dirty = true;
        }
    }
}
