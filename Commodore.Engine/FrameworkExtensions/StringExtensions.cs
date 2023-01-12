using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Commodore.Engine.FrameworkExtensions
{
    public static class StringExtensions
    {
        private readonly static char[] _glitchCharacters = "~`!@#$%^&*()_-+={[}]:;\"'\\|,<.>/?".ToCharArray();

        public static string[] SplitAt(this string source, params int[] index)
        {
            index = index.Distinct().OrderBy(x => x).ToArray();
            string[] output = new string[index.Length + 1];
            int pos = 0;

            for (int i = 0; i < index.Length; pos = index[i++])
                output[i] = source.Substring(pos, index[i] - pos);

            output[index.Length] = source.Substring(pos);
            return output;
        }

        public static List<string> SplitEvery(this string s, int n)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            if (n <= 0)
                throw new ArgumentException($"'{nameof(n)}' cannot be negative.");

            var l = new List<string>();

            for (var i = 0; i < s.Length; i += n)
                l.Add(s.Substring(i, System.Math.Min(n, s.Length - i)));

            return l;
        }

        public static string Glitched(this string str, int insertChance = 5, int replaceChance = 10)
        {
            var sb = new StringBuilder();

            foreach (var c in str)
            {
                if (c == '\n')
                {
                    sb.Append(c);
                    continue;
                }

                if (G.Random.Next(0, 100) < insertChance)
                {
                    sb.Append(c);
                    sb.Append(
                        _glitchCharacters[G.Random.Next(0, _glitchCharacters.Length)]
                    );

                    continue;
                }

                if (G.Random.Next(0, 100) < replaceChance)
                {
                    sb.Append(
                        _glitchCharacters[G.Random.Next(0, _glitchCharacters.Length)]
                    );

                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}