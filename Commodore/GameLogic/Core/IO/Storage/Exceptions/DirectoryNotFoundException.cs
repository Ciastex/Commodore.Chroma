using System;

namespace Commodore.GameLogic.Core.IO.Storage.Exceptions
{
    public class DirectoryNotFoundException : Exception
    {
        public string Name { get; }

        public DirectoryNotFoundException(string name, string message) : base(message)
        {
            Name = name;
        }
    }
}
