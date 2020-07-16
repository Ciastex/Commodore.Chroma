using Commodore.Framework;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public class Window
    {
        private Editor Editor { get; }

        public uint LeftMargin { get; } = 1;
        public uint TopMargin { get; } = 1;
        public uint RightMargin { get; } = 1;
        public uint BottomMargin { get; } = 2;

        public int TotalColumns { get; set; }
        public int TotalRows { get; set; }

        public int Top { get; set; }
        public int Bottom { get; set; }

        public Window(Editor editor)
        {
            Editor = editor;
            Reset();
        }

        public void Reset()
        {
            TotalColumns = G.Window.Size.Width / Editor.TextRenderer.HorizontalGranularity - (int)(LeftMargin + RightMargin);
            TotalRows = G.Window.Size.Height / Editor.TextRenderer.VerticalGranularity - (int)(TopMargin + BottomMargin);

            Top = 0;
            Bottom = TotalRows;
        }

        public bool MoveUp()
        {
            if (Top <= 0)
                return false;

            Top--;
            Bottom--;

            return true;
        }

        public bool MoveDown()
        {
            if (Bottom >= Editor.Buffer.Lines.Count || Editor.Buffer.Lines.Count < TotalRows)
                return false;

            Top++;
            Bottom++;

            return true;
        }
    }
}
