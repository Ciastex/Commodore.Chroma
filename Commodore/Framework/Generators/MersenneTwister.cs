using System;

namespace Commodore.Framework.Generators
{
    [Serializable]
    public class MersenneTwister
    {
        private const int W = 32;
        private const int N = 624;
        private const int M = 397;
        private const int R = 31;
        private const int U = 11;
        private const int S = 7;
        private const int T = 15;
        private const int L = 18;
        private const uint A = 0x9908B0DF;
        private const uint B = 0x9D2C5680;
        private const uint C = 0xEFC60000;
        private const uint D = 0xFFFFFFFF;
        private const uint F = 1812433253;

        private const uint LowerMask = (1u << R) - 1;
        private static readonly uint UpperMask = ~LowerMask & (uint)((long)System.Math.Pow(2, W) - 1);

        private int _seed;
        private int _index = N;

        private readonly int[] MT = new int[N];

        public int Seed
        {
            get => _seed;
            set
            {
                _index = N;
                _seed = value;

                MT[0] = value;

                for (var i = 1; i < N; i++)
                {
                    MT[i] = (int)(F * (MT[i - 1] ^ (MT[i - 1] >> (W - 2))) + i);
                }
            }
        }

        public MersenneTwister()
        {
            Seed = (int)DateTime.Now.Ticks;
        }

        public MersenneTwister(int seed)
        {
            Seed = seed;
        }

        public int Next(int min, int max)
        {
            return (Next() % (max - min + 1)) + min;
        }

        public int Next()
        {
            if (_index >= N)
            {
                Twist();
            }

            var y = MT[_index];
            y ^= (int)((y >> U) & D);
            y ^= (int)((y << S) & B);
            y ^= (int)((y << T) & C);
            y ^= (y >> L);

            _index++;
            var mask = (int)(((long)System.Math.Pow(2, W)) - 1);

            return y & mask;
        }

        private void Twist()
        {
            for (var i = 0; i < N; i++)
            {
                var x = (int)(MT[i] & UpperMask) + (MT[(i + 1) % N] & LowerMask);
                var xA = (int)(x >> 1);

                if (x % 2 != 0)
                {
                    xA = (int)(xA ^ A);
                }

                MT[i] = MT[(i + M) % N] ^ xA;
            }

            _index = 0;
        }
    }
}
