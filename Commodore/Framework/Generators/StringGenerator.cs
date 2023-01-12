using System.Text;

namespace Commodore.Framework.Generators
{
    public class StringGenerator
    {
        private static readonly char[] _alphanumericAlphabet =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        private static readonly char[] _hexadecimalAlphabet = "0123456789ABCDEF".ToCharArray();

        public static string GenerateRandomString(char[] alphabet, int length)
        {
            if (alphabet.Length == 0 || length == 0)
                return string.Empty;

            var sb = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var c = alphabet[G.Random.Next(0, alphabet.Length - 1)];
                sb.Append(c);
            }

            return sb.ToString();
        }

        public static string GenerateRandomAlphanumericString(int length) =>
            GenerateRandomString(_alphanumericAlphabet, length);

        public static string GenerateRandomHexadecimalString(int length) =>
            GenerateRandomString(_hexadecimalAlphabet, length);
    }
}