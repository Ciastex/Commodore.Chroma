using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chroma.Input;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Execution;
using Commodore.EVIL.RuntimeLibrary.Base;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Core.IO.Storage;
using Environment = Commodore.EVIL.Environment;

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
                    prompt,
                    Kernel.Instance.InteractionCancellation.Token
                )
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
                    prompt,
                    Kernel.Instance.InteractionCancellation.Token
                )
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

        public DynValue WaitForProcess(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectIntegerAtIndex(0);

            interpreter.SuspendExecution = true;
            Kernel.Instance.ProcessManager.WaitForProgram((int)args[0].Number, Kernel.Instance.InteractionCancellation.Token).GetAwaiter().GetResult();
            interpreter.SuspendExecution = false;

            return DynValue.Zero;
        }

        public DynValue Import(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectExactly(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var libName = args[0].String;

            var path = "/lib/" + libName + ".lib";
            var file = File.Get(path);
            var absPath = file.GetAbsolutePath();

            var process = Kernel.Instance.ProcessManager.GetProcess(interpreter);

            if (process.ImportedLibraryPaths.Contains(absPath))
            {
                interpreter.TerminateWithKernelMessage($"{absPath} WAS ALREADY IMPORTED");
                return new DynValue(-1);
            }

            try
            {
                var content = file.GetData();
                process.ImportedLibraryPaths.Add(absPath);

                return interpreter.ExecuteAsync(content, Kernel.Instance.InteractionCancellation.Token).GetAwaiter().GetResult();
            }
            catch
            {
                interpreter.TerminateWithKernelMessage($"FAILED TO IMPORT {absPath}");
                return new DynValue(-2);
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

        public DynValue SpawnProcess(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectAtLeast(1)
                .ExpectTypeAtIndex(0, DynValueType.String);

            var path = args[0].String;

            if (!File.Exists(path, true))
            {
                return new DynValue(SystemReturnCodes.FileSystem.FileDoesNotExist);
            }

            var file = File.Get(path, true);
            if ((file.Attributes & FileAttributes.Executable) == 0)
            {
                return new DynValue(SystemReturnCodes.FileSystem.AccessDenied);
            }

            var processArgs = new List<string>();
            if (args.Count > 1)
            {
                for (var i = 1; i < args.Count; i++)
                {
                    args.ExpectTypeAtIndex(i, DynValueType.String);
                    processArgs.Add(args[i].String);
                }
            }

            var pid = Kernel.Instance.ProcessManager.ExecuteProgram(file.GetData(), path, Kernel.Instance.InteractionCancellation.Token, processArgs.ToArray());

            if (pid < 0)
                return new DynValue(SystemReturnCodes.Kernel.ProcessSpaceExhausted);

            var tbl = new Table();
            tbl["pid"] = new DynValue(pid);
            tbl["path"] = new DynValue(path);

            return new DynValue(tbl);
        }

        public DynValue LastReturnValue(Interpreter interpreter, ClrFunctionArguments args)
        {
            args.ExpectNone();

            if (!Kernel.Instance.ProcessManager.ReturnValues.TryPeek(out var value))
                return DynValue.Zero;

            return value;
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
            env.RegisterBuiltIn("sys.exec", SpawnProcess);
            env.RegisterBuiltIn("sys.waitproc", WaitForProcess);
            env.RegisterBuiltIn("sys.retval", LastReturnValue);
        }
    }
}