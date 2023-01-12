using Chroma.Graphics;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public class EditorColors
    {
        public static Color EditorBackground = Color.Black;
        public static Color EditorForeground = Color.DarkGray;
        public static Color EditorCursorColor = new Color(120, 120, 120, 63);
        public static Color EditorLineHighlightColor = new Color(66, 66, 66, 63);

        public static Color ScrollBarForeground = Color.DarkGray;
        public static Color ScrollBarBackground = new Color(11, 11, 11, 95);

        public static Color ModeLineBackground = Color.DarkGray;
        public static Color ModeLineForeground = Color.Black;

        public static Color Keyword = Color.DeepSkyBlue;
        public static Color ValueWord = Color.BlueViolet;
        public static Color Operator = Color.DimGray;
        public static Color String = Color.Orange;
        public static Color Number = Color.Salmon;
        public static Color Comment = Color.DarkGreen;
    }
}
