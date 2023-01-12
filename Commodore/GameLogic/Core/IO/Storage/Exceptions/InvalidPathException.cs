using System;

namespace Commodore.GameLogic.Core.IO.Storage.Exceptions
{
    public class InvalidPathException : Exception
    {
        public string Path { get; }

        public InvalidPathException(string path, string message) : base(message)
        {
            Path = path;
        }
    }
}
