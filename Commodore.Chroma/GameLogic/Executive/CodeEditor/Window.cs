using Commodore.Engine;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public class Window
    {
        private Editor Editor { get; }

        public Margins Margins { get; }

        public int TotalColumns { get; set; }
        public int TotalRows { get; set; }

        public int Top { get; set; }
        public int Bottom { get; set; }

        public Window(Editor editor)
        {
            Margins = new Margins
            {
                Left = 1,
                Top = 1,
                Right = 1,
                Bottom = 2
            };

            Editor = editor;
            Reset();
        }

        public void Reset()
        {
            TotalColumns = G.Window.Size.Width / Editor.TextRenderer.HorizontalGranularity - Margins.Left - Margins.Right;
            TotalRows = G.Window.Size.Height / Editor.TextRenderer.VerticalGranularity - Margins.Top - Margins.Bottom;

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
