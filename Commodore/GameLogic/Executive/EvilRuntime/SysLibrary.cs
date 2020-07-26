using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chroma.Input;
using Commodore.EVIL;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Core.IO.Storage;

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

        public DynValue KeyDown(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.Number);

            var keyCode = args[0].Number;

            return new DynValue(Keyboard.IsKeyDown((KeyCode)keyCode) ? 1 : 0);
        }
        
        public DynValue KeyUp(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.Number);

            var keyCode = args[0].Number;

            return new DynValue(Keyboard.IsKeyUp((KeyCode)keyCode) ? 1 : 0);
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

        public DynValue Pid(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();
            return new DynValue(Kernel.Instance.ProcessManager.GetPid(interpreter));
        }

        public DynValue ProcList(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            var tbl = new Table();
            var sb = new StringBuilder();
            
            for (var i = 0; i < Kernel.Instance.ProcessManager.Processes.Values.Count; i++)
            {
                var proc = Kernel.Instance.ProcessManager.Processes.Values.ElementAt(i);

                sb.Append("[");
                sb.Append(proc.Pid);
                sb.Append("] ");
                sb.Append(proc.FilePath);
                sb.Append(" ");
                sb.Append(proc.CommandLine);

                tbl[i] = new DynValue(sb.ToString());
                sb.Clear();
            }

            return new DynValue(tbl);
        }

        public DynValue Kill(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectIntegerAtIndex(0);

            var pid = (int)args[0].Number;

            if (Kernel.Instance.ProcessManager.Processes.ContainsKey(pid))
            {
                Kernel.Instance.ProcessManager.Kill(pid);
                return new DynValue(1);
            }
            else
            {
                return DynValue.Zero;
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
            env.RegisterBuiltIn("sys.keyup", KeyUp);
            env.RegisterBuiltIn("sys.keydown", KeyDown);
            
            env.RegisterBuiltIn("sys.pid", Pid);
            env.RegisterBuiltIn("sys.proclist", ProcList);
            env.RegisterBuiltIn("sys.kill", Kill);
        }
    }
}
