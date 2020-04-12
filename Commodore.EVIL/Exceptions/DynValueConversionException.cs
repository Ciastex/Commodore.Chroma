using System;

namespace Commodore.EVIL.Exceptions
{
    public class DynValueConversionException : Exception
    {
        public DynValueConversionException(string message) : base(message)
        {
        }
    }
}