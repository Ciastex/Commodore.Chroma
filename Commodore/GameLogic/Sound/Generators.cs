using Commodore.Framework;
using Math = System.Math;

namespace Commodore.GameLogic.Sound
{
    public static class Generators
    {
        public static float Noise(float frequency, float time) =>
            (float)G.Random.NextDouble();

        public static float SawTooth(float frequency, float time) =>
            (float)(2 * (time * frequency - Math.Floor(time * frequency + 0.5)));

        public static float Sine(float frequency, float time) =>
            (float)Math.Sin(frequency * time * 2 * Math.PI);

        public static float Square(float frequency, float time) =>
            Math.Sin(frequency * time * 2 * Math.PI) > 0 ? 1f : -1f;

        public static float Triangle(float frequency, float time) =>
            (float)(Math.Abs(2 * (time * frequency - Math.Floor(time * frequency + 0.5))) * 2.0f - 1.0f);
    }
}