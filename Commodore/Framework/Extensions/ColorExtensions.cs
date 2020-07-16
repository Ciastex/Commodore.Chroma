using Chroma.Graphics;

namespace Commodore.Framework.Extensions
{
    public static class ColorExtensions
    {
        public static Color Subtract(this Color color, byte v)
        {
            return new Color(
                (byte)(color.R - v),
                (byte)(color.G - v),
                (byte)(color.B - v)
            );
        }

        public static Color Add(this Color color, byte v)
        {
            return new Color(
                (byte)(color.R + v),
                (byte)(color.G + v),
                (byte)(color.B + v)
            );
        }
    }
}
