using System.Text;

namespace Commodore.Engine.Generators
{
    public class StringGenerator
    {
        private static readonly char[] _alphanumericAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        private static readonly char[] _hexadecimalAlphabet = "0123456789ABCDEF".ToCharArray();

        public static string GenerateRandomString(char[] alphabet, int length, MersenneTwister random = null)
        {
            if (alphabet.Length == 0 || length == 0)
                return string.Empty;

            var sb = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                char c;
                if (random == null)
                {
                    c = alphabet[G.Random.Next(alphabet.Length - 1)];
                }
                else
                {
                    c = alphabet[random.Next(0, alphabet.Length - 1)];
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        public static string GenerateRandomAlphanumericString(int length, MersenneTwister random) => GenerateRandomString(_alphanumericAlphabet, length, random);
        public static string GenerateRandomHexadecimalString(int length, MersenneTwister random) => GenerateRandomString(_hexadecimalAlphabet, length, random);
    }
}
