using Commodore.EVIL;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Core.IO.Storage;
using System.Threading.Tasks;

namespace Commodore.GameLogic.Executive.EvilRuntime
{
    public class SysLibrary : ClrPackage
    {
        public DynValue Print(Interpreter interpreter, ClrFunctionArguments args)
        {
            for (var i = 0; i < args.Count; i++)
            {
                var value = args[i];

                Kernel.Instance.Terminal.Write(value.AsString().String);

                if (i != args.Count - 1)
                    Kernel.Instance.Terminal.Write(" ");
            }

            return DynValue.Zero;
        }

        public DynValue PrintLine(Interpreter interpreter, ClrFunctionArguments args)
        {
            foreach (var value in args)
                Kernel.Instance.Terminal.WriteLine(value.AsString().String);

            return DynValue.Zero;
        }

        public DynValue ReadKey(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectAtMost(1);

            var prompt = string.Empty;
            if (args.Count == 1)
            {
                args.ExpectTypeAtIndex(0, DynValueType.String);
                prompt = args[0].String;
            }

            return new DynValue(
                Kernel.Instance.Terminal.Read(
                    prompt
                ).GetAwaiter().GetResult()
            );
        }

        public DynValue ReadLine(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectAtMost(1);

            var prompt = string.Empty;
            if (args.Count == 1)
            {
                args.ExpectTypeAtIndex(0, DynValueType.String);
                prompt = args[0].String;
            }

            return new DynValue(
                Kernel.Instance.Terminal.ReadLine(
                    prompt
                ).GetAwaiter().GetResult()
            );
        }

        public DynValue Wait(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectIntegerAtIndex(0);

            Task.Delay((int)args[0].Number).GetAwaiter().GetResult();
            return DynValue.Zero;
        }

        public DynValue Import(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var libName = args[0].String;

            try
            {
                var content = File.Get("/lib/" + libName + ".lib").GetData();
                return interpreter.ExecuteAsync(content).GetAwaiter().GetResult();
            }
            catch
            {
                interpreter.TerminateWithKernelMessage($"FAILED TO IMPORT /lib/{libName}.lib");
                return new DynValue(-1);
            }
        }

        public override void Register(Environment env, Interpreter interpreter)
        {
            env.RegisterBuiltIn("import", Import);

            env.RegisterBuiltIn("sys.print", Print);
            env.RegisterBuiltIn("sys.println", PrintLine);
            env.RegisterBuiltIn("sys.readkey", ReadKey);
            env.RegisterBuiltIn("sys.readln", ReadLine);
            env.RegisterBuiltIn("sys.wait", Wait);
        }
    }
}
