namespace Commodore.GameLogic.Executive.CodeEditor
{
    public partial class Editor
    {
        public void CursorRight()
        {
            if (Buffer.IsEndOfLine)
            {
                if (Buffer.CurrentLineNumber + 1 >= Buffer.Lines.Count)
                    return;

                var moveCursor = LineDown();
                if (Buffer.CursorY < Window.TotalRows - 1)
                {
                    Buffer.CursorX = 0;
                    Buffer.CursorY++;
                }
                else
                {
                    if (moveCursor)
                        Buffer.CursorX = 0;
                }
            }
            else
            {
                Buffer.CursorX++;
            }
        }

        public void CursorLeft()
        {
            if (Buffer.CursorX == 0)
            {
                var moveCursor = LineUp();

                if (Buffer.CursorY > 0)
                {
                    Buffer.CursorX = Buffer.CurrentLineLength;
                    Buffer.CursorY--;
                }
                else
                {
                    if (moveCursor)
                        Buffer.CursorX = Buffer.CurrentLineLength;
                }
            }
            else
            {
                Buffer.CursorX--;
            }
        }

        public void CursorDown(bool forceLineEnd = false, bool skipLineEnd = false, bool skipLineDown = false)
        {
            if (Buffer.CursorY < Window.TotalRows - 1)
            {
                if (Buffer.CurrentLineNumber < Buffer.Lines.Count - 1)
                    Buffer.CursorY++;
            }

            if (!skipLineDown)
                LineDown();

            var currentLineEnd = Buffer.CurrentLineLength;

            if ((Buffer.CursorX > currentLineEnd || forceLineEnd) && !skipLineEnd)
                Buffer.CursorX = currentLineEnd;
        }

        public void CursorUp(bool forceLineEnd = false, bool skipLineUp = false)
        {
            if (Buffer.CursorY > 0)
                Buffer.CursorY--;

            if (!skipLineUp)
                LineUp();

            var currentLineEnd = Buffer.CurrentLineLength;

            if (Buffer.CursorX > currentLineEnd || forceLineEnd)
                Buffer.CursorX = currentLineEnd;
        }
    }
}
