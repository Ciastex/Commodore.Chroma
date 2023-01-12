using System;

namespace Commodore.GameLogic.Core.Exceptions
{
    public class KernelException : Exception
    {
        public KernelException(string message) : base(message) { }
    }
}
