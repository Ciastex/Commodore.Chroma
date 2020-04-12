using System.Collections.Generic;
using System.Numerics;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public class TextRenderer
    {
        private Editor Editor { get; }

        private TrueTypeFont Font { get; }

        private List<string> LineBuffer { get; }
        private TextColorizer TextColorizer { get; }

        public int HorizontalGranularity { get; set; } = 16;
        public int VerticalGranularity { get; set; } = 16;

        public bool DisableSyntaxHighlighter { get; set; }

        public TextRenderer(Editor editor)
        {
            Editor = editor;
            Font = editor.Font;
            TextColorizer = new TextColorizer();

            LineBuffer = new List<string>();
        }

        public void Draw(RenderContext context)
        {
            for (var i = Editor.Window.Top; i < Editor.Window.Bottom; i++)
            {
                if (i >= LineBuffer.Count)
                    break;

                var linePosition = Editor.CalculateLinePosition(i - Editor.Window.Top);

                if (DisableSyntaxHighlighter)
                {
                    context.DrawString(
                        Font,
                        LineBuffer[i],
                        linePosition,
                        (c, i, p, g) => new GlyphTransformData(p)
                        {
                            Color = EditorColors.EditorForeground
                        });
                }
                else
                {
                    var colorizedSegments = TextColorizer.Colorize(LineBuffer[i]);

                    var currentOffsetX = 0;
                    for (var j = 0; j < colorizedSegments.Count; j++)
                    {
                        context.DrawString(
                            Font,
                            colorizedSegments[j].Value,
                            new Vector2(
                                linePosition.X + currentOffsetX,
                                linePosition.Y
                            ),
                            (c, i, p, g) => new GlyphTransformData(p)
                            {
                                Position = new Vector2(p.X, p.Y),
                                Color = colorizedSegments[j].Color
                            }
                        );

                        currentOffsetX += (colorizedSegments[j].Value).Length * VerticalGranularity;
                    }
                }
            }
        }

        public void PushLines(List<string> lines)
        {
            LineBuffer.Clear();
            LineBuffer.AddRange(lines);
        }
    }
}