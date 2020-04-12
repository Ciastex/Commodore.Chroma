using System.Numerics;

namespace Commodore.Engine
{
    public static class Math
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value > max) return max;
            if (value < min) return min;

            return value;
        }

        public static float Lerp(float from, float to, float pos)
        {
            pos = Clamp(pos, 0f, 1f);

            return from * (1 - pos) + to * pos;
        }

        public static Vector2 Lerp(Vector2 from, Vector2 to, float pos)
        {
            pos = Clamp(pos, 0f, 1f);

            float retX = Lerp(from.X, to.X, pos);
            float retY = Lerp(from.Y, to.Y, pos);

            return new Vector2(retX, retY);
        }

        public static double DistanceBetween(double x1, double y1, double x2, double y2)
        {
            var a = x1 - x2;
            var b = y1 - y2;

            return System.Math.Sqrt(
                System.Math.Pow(a, 2) +
                System.Math.Pow(b, 2)
            );
        }
    }
}
