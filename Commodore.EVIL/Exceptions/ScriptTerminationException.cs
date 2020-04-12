using System;

namespace Commodore.EVIL.Exceptions
{
    public class ScriptTerminationException : Exception
    {
        public ScriptTerminationException(string message) : base(message)
        {

        }
    }
}
