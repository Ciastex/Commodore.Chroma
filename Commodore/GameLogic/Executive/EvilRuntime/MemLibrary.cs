using Commodore.EVIL;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;

namespace Commodore.GameLogic.Executive.EvilRuntime
{
    public class MemLibrary : ClrPackage
    {
        public DynValue Size(Interpreter interpreter, ClrFunctionArguments args)
        {
            return new DynValue(SystemConstants.MemorySize * SystemConstants.MemoryPlanes);
        }

        public override void Register(Environment env, Interpreter interpreter)
        {
            env.RegisterBuiltIn("mem.size", Size);
        }
    }
}
