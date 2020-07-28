using Commodore.GameLogic.Core.IO.Storage.Exceptions;
using System.Linq;

namespace Commodore.GameLogic.Core.IO.Storage
{
    public static class Path
    {
        public static readonly char[] InvalidPathCharacters = new char[0];

        public static bool IsAbsolute(string path)
            => path.StartsWith("/");

        public static bool ContainsInvalidCharacters(string path)
            => InvalidPathCharacters.Intersect(path).Count() != 0;

        public static string GetDirectoryName(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new InvalidPathException(path, "Provided path was empty.");

            var absolute = IsAbsolute(path);
            var segments = path.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (segments.Length == 0)
                return "/";

            if (segments.Length == 1)
                return Kernel.Instance.FileSystemContext.WorkingDirectory.GetAbsolutePath();

            var withoutFile = segments.Take(segments.Length - 1);

            var ret = string.Join("/", withoutFile);

            if (absolute)
                ret = "/" + ret;

            return ret;
        }

        public static string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new InvalidPathException(path, "Provided path was empty.");

            var segments = path.Split('/').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (segments.Length == 0)
                throw new InvalidPathException(path, "Provided path is too short.");

            return segments[segments.Length - 1];
        }
    }
}
