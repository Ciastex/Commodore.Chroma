namespace Commodore.GameLogic.Executive.CodeEditor
{
    public partial class Editor
    {
        public void NewLine()
        {
            if (Buffer.CursorX == 0)
            {
                Buffer.Lines.Insert(Buffer.CurrentLineNumber, string.Empty);
                CursorDown(false, true, true);
                LineDown();
            }
            else if (Buffer.IsEndOfLine)
            {
                Buffer.Lines.Insert(Buffer.CurrentLineNumber + 1, string.Empty);

                CursorDown(false, true, true);
                LineDown();

                Buffer.CursorX = 0;
            }
            else
            {
                var preCursor = Buffer.PreCursorSubstring;
                var postCursor = Buffer.PostCursorSubstring;

                Buffer.Lines.Insert(Buffer.CurrentLineNumber + 1, string.Empty);

                Buffer.Lines[Buffer.CurrentLineNumber] = preCursor;
                Buffer.Lines[Buffer.CurrentLineNumber + 1] = postCursor;

                CursorDown(false, true, true);
                LineDown();

                Buffer.CursorX = 0;
            }

            Buffer.Dirty = true;
        }

        public void Backspace()
        {
            if (Buffer.Lines[Buffer.CurrentLineNumber].Length == 0)
            {
                if (Buffer.CurrentLineNumber == 0)
                    return;

                var indexToRemove = Buffer.CurrentLineNumber;
                LineUp();
                CursorUp(true, true);

                Buffer.Lines.RemoveAt(indexToRemove);
            }
            else if (Buffer.CursorX == 0)
            {
                if (Buffer.CurrentLineNumber == 0)
                    return;

                if (Buffer.Lines[Buffer.CurrentLineNumber - 1].Length != 0)
                {
                    var previousLine = Buffer.CurrentLineNumber;

                    LineUp();
                    CursorUp(true, true);

                    Buffer.Lines[Buffer.CurrentLineNumber] = Buffer.Lines[Buffer.CurrentLineNumber].Substring(0, Buffer.Lines[Buffer.CurrentLineNumber].Length);
                    Buffer.Lines[Buffer.CurrentLineNumber] += Buffer.Lines[previousLine];

                    Buffer.Lines.RemoveAt(previousLine);

                    return;
                }
                else
                {
                    var indexToRemove = Buffer.CurrentLineNumber - 1;
                    LineUp();
                    CursorUp(true, true);

                    Buffer.Lines.RemoveAt(indexToRemove);
                }
            }
            else
            {
                var preCursor = Buffer.PreCursorSubstring;
                var postCursor = Buffer.PostCursorSubstring;

                preCursor = preCursor.Substring(0, preCursor.Length - 1);

                Buffer.Lines[Buffer.CurrentLineNumber] = preCursor + postCursor;
                CursorLeft();
            }

            Buffer.Dirty = true;
        }

        public void Delete()
        {
            if (Buffer.Lines[Buffer.CurrentLineNumber].Length == 0)
            {
                if (Buffer.CurrentLineNumber + 1 >= Buffer.Lines.Count)
                    return;

                Buffer.Lines.RemoveAt(Buffer.CurrentLineNumber);
            }
            else if (Buffer.CursorX == 0)
            {
                if (Buffer.Lines[Buffer.CurrentLineNumber].Length == 0)
                    return;

                Buffer.Lines[Buffer.CurrentLineNumber] = Buffer.Lines[Buffer.CurrentLineNumber].Substring(Buffer.CursorX + 1);
            }
            else if (Buffer.IsEndOfLine)
            {
                if (Buffer.CurrentLineNumber + 1 >= Buffer.Lines.Count)
                    return;

                var preCursor = Buffer.Lines[Buffer.CurrentLineNumber + 1];

                Buffer.Lines[Buffer.CurrentLineNumber] += preCursor;
                Buffer.Lines.RemoveAt(Buffer.CurrentLineNumber + 1);
                // save next line's text, delete next line, join it with the current line, don't move the cursor
            }
            else
            {
                var preCursor = Buffer.PreCursorSubstring;
                var postCursor = Buffer.PostCursorSubstringWithoutFirstCharacter;

                Buffer.Lines[Buffer.CurrentLineNumber] = preCursor + postCursor;
                // split the current line, remove second split product's first character, join back, don't move the cursor
            }

            Buffer.Dirty = true;
        }
    }
}
