using Commodore.EVIL.Lexical;
using System;

namespace Commodore.EVIL.Exceptions
{
    public class ParserException : Exception
    {
        public ScannerState ScannerState { get; }

        public ParserException(string message) : base(message) { }

        public ParserException(string message, ScannerState scannerState) : base(message)
            => ScannerState = scannerState;
    }
}
