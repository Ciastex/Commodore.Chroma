using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using System;

namespace Commodore.EVIL.RuntimeLibrary
{
    public class TimeLibrary : ClrPackage
    {
        public DynValue Stamp(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            var stamp = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            return new DynValue(stamp);
        }

        public override void Register(Environment env, Interpreter interpreter)
        {
            env.RegisterBuiltIn("time.stamp", Stamp);
        }
    }
}
