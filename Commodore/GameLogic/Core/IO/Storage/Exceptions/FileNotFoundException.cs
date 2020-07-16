using System;

namespace Commodore.GameLogic.Core.IO.Storage.Exceptions
{
    public class FileNotFoundException : Exception
    {
        public string Name { get; }

        public FileNotFoundException(string name, string message) : base(message)
        {
            Name = name;
        }
    }
}
