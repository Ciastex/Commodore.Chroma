using System;

namespace Commodore.GameLogic.Sound
{
    public static class Utilities
    {
        public static float NoteToFrequency(int note) =>
            (float)(440f * Math.Pow(2, (note - 9) / 12f));

        public static float MixSamples(float a, float b)
        {
            if (a < 0 && b < 0)
                return (a + b) - ((a * b) / -1f);

            if (a > 0 && b < 0)
                return (a + b) - ((a * b) / 1f);

            return a + b;
        }

        /* REWRITE ARTIFACT */
        /*
        public static void WriteToXnaBuffer(float[,] from, byte[] to)
        {
            var channels = from.GetLength(0);
            var bufferSize = from.GetLength(1);

            for(var i = 0; i < bufferSize; i++)
            {
                for(var j = 0; j < channels; j++)
                {
                    var floatSample = Engine.Math.Clamp(from[j, i], -1f, 1f);
                    var shortSample = (short)(floatSample >= 0 ? floatSample * short.MaxValue : floatSample * short.MinValue * -1);

                    var index = i * channels * Synthesizer.BytesPerSample + j * Synthesizer.BytesPerSample;

                    to[index] = (byte)shortSample;
                    to[index + 1] = (byte)(shortSample >> 8);
                }
            }
        }*/
    }
}
