using System.Collections.Generic;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public class EditorBuffer
    {
        public int CursorX { get; set; }
        public int CursorY { get; set; }

        public int CurrentLineNumber { get; set; }

        public bool Dirty { get; set; }
        public string CurrentFileName { get; set; }

        public List<string> Lines { get; set; }
        public string LastKill { get; set; }

        public string CurrentLine => Lines[CurrentLineNumber];

        public string PreviousLine
        {
            get
            {
                if (CurrentLineNumber - 1 < 0)
                    return null;

                return Lines[CurrentLineNumber - 1];
            }
        }

        public string NextLine
        {
            get
            {
                if (CurrentLineNumber + 1 >= Lines.Count)
                    return null;

                return Lines[CurrentLineNumber + 1];
            }
        }

        public string PreCursorSubstring => Lines[CurrentLineNumber].Substring(0, CursorX);
        public string PostCursorSubstring => Lines[CurrentLineNumber].Substring(CursorX);
        public string PostCursorSubstringWithoutFirstCharacter => Lines[CurrentLineNumber].Substring(CursorX).Substring(1);

        public int CurrentLineLength => Lines[CurrentLineNumber].Length;
        public bool IsEndOfLine => CursorX >= Lines[CurrentLineNumber].Length;

        public EditorBuffer()
        {
            Reset();
        }

        public void Reset()
        {
            CursorX = 0;
            CursorY = 0;

            Lines = new List<string>();

            Dirty = false;

            CurrentLineNumber = 0;
            CurrentFileName = null;

            Lines.Clear();
            Lines.Add(string.Empty);

            LastKill = string.Empty;
        }
    }
}
