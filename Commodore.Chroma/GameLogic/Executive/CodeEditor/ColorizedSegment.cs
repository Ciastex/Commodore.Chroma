using Chroma.Graphics;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public class ColorizedSegment
    {
        public Color Color { get; set; } = EditorColors.EditorForeground;
        public string Value { get; set; } = string.Empty;
    }
}
