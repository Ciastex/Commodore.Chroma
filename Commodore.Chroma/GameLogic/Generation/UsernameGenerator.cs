using Commodore.Engine;
using Commodore.Engine.Generators;
using System.Collections.Generic;
using System.Linq;

namespace Commodore.GameLogic.Generation
{
    public static class UsernameGenerator
    {
        private static readonly char[] _fallbackAlphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly List<string> _usernames = new List<string>();

        public static void Add(string username)
            => _usernames.Add(username);

        public static void AddRange(IEnumerable<string> usernames)
            => _usernames.AddRange(usernames);

        public static string Generate(MersenneTwister random)
        {
            if (_usernames.Any())
                return _usernames[random.Next(0, _usernames.Count - 1)];

            DebugLog.Warning("Generating random username as the predefined list is empty.");
            return StringGenerator.GenerateRandomString(_fallbackAlphabet, 6, random);
        }
    }
}
