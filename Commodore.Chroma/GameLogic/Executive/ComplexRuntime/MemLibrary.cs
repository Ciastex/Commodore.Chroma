using Commodore.EVIL;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;

namespace Commodore.GameLogic.Executive.EVILRuntime
{
    public class MemLibrary : ClrPackage
    {
        public DynValue SetPlane(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectIntegerAtIndex(0);

            if (args[0].Number > 255)
                return new DynValue(-2);

            var planeNum = (byte)args[0].Number;

            if (planeNum >= Kernel.Instance.Memory.Planes.Count)
                return new DynValue(-1);

            if (planeNum < 0)
                return new DynValue(-1);

            Kernel.Instance.Memory.SetPlane(planeNum);

            return DynValue.Zero;
        }

        public DynValue Size(Interpreter interpreter, ClrFunctionArguments args)
        {
            return new DynValue(SystemConstants.MemorySize * SystemConstants.MemoryPlanes);
        }

        public DynValue Planes(Interpreter interpreter, ClrFunctionArguments args)
        {
            return new DynValue(Kernel.Instance.Memory.Planes.Count);
        }

        public DynValue PlaneSize(Interpreter interpreter, ClrFunctionArguments args)
        {
            return new DynValue(SystemConstants.MemorySize);
        }

        public DynValue CurrentPlane(Interpreter interpreter, ClrFunctionArguments args)
        {
            return new DynValue(Kernel.Instance.Memory.CurrentPlane);
        }

        public override void Register(Environment env, Interpreter interpreter)
        {
            env.RegisterBuiltIn("mem.setplane", SetPlane);
            env.RegisterBuiltIn("mem.size", Size);
            env.RegisterBuiltIn("mem.planesize", PlaneSize);
            env.RegisterBuiltIn("mem.planes", Planes);
            env.RegisterBuiltIn("mem.curplane", CurrentPlane);
        }
    }
}
