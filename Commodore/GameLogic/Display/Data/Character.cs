using Chroma.Graphics;

namespace Commodore.GameLogic.Display.Data
{
    public struct Character
    {
        public char Value;
        public Color Foreground;
        public Color Background;

        public Character(char value, Color foreground)
        {
            Value = value;
            Foreground = foreground;
            Background = Color.Transparent;
        }

        public Character(char value, Color foreground, Color background)
        {
            Value = value;
            Foreground = foreground;
            Background = background;
        }

        public void Reset()
        {
            Value = ' ';
            Foreground = Color.White;
            Background = Color.Transparent;
        }
    }
}
