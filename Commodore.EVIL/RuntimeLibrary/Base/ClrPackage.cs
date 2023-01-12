using Commodore.EVIL.Execution;

namespace Commodore.EVIL.RuntimeLibrary.Base
{
    public abstract class ClrPackage
    {
        public abstract void Register(Environment env, Interpreter interpreter);

        public virtual void Reset(Environment env, Interpreter interpreter)
        {

        }
    }
}
