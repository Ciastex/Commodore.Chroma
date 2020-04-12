using System;

namespace Commodore.EVIL.Exceptions
{
    public class RuntimeException : Exception
    {
        public int? Line { get; }

        public RuntimeException(string message, int? line) : base(message)
        {
            Line = line;
        }
    }
}
