using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Chroma.Diagnostics.Logging;
using Commodore.Framework;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Exceptions;
using Commodore.EVIL.Execution;
using Commodore.GameLogic.Core.Exceptions;
using Commodore.GameLogic.Executive.EvilRuntime;

namespace Commodore.GameLogic.Core
{
    public class ProcessManager
    {
        public const int MaximumProcessCount = 64;
        
        private Log Log { get; } = LogManager.GetForCurrentAssembly();
        private int _nextPid;
        
        public Dictionary<int, Process> Processes { get; private set; }

        public ProcessManager() => Reset();

        public void Reset()
        {
            if (Processes != null)
            {
                KillAll();
            }
            else
            {
                Processes = new Dictionary<int, Process>();
            }
        }

        public void Kill(int pid)
        {
            if (!Processes.ContainsKey(pid))
                return;

            Processes[pid].Interpreter.BreakExecution = true;
        }

        public void KillAll()
        {
            for (var i = 0; i < Processes.Values.Count; i++)
                Processes.Values.ElementAt(i).Interpreter.BreakExecution = true;
        }

        public Process GetProcess(Interpreter interpreter)
        {
            return Processes.Values.FirstOrDefault(p => p.Interpreter == interpreter);
        }

        public int GetPid(Interpreter interpreter)
        {
            var proc = GetProcess(interpreter);
            
            if (proc == null)
                return -1;

            return proc.Pid;
        }

        public async Task WaitForProgram(int pid)
        {
            while (Processes.ContainsKey(pid))
                await Task.Delay(1);
        }

        public async Task<int> ExecuteProgram(string code, string filePath, params string[] args)
        {
            if (Processes.Count >= MaximumProcessCount)
                return -1;
            
            var interp = CreateProcess();
            var targetCode = code ?? string.Empty;

            using (var fs = G.ContentProvider.Open("Sources/Templates/program.template.cplx"))
            {
                using (var sr = new StreamReader(fs))
                    targetCode = (await sr.ReadToEndAsync()).Replace("%prepped_source%", targetCode);
            }

            var argsTable = new Table();
            for (var i = 0; i < args.Length; i++)
                argsTable[i] = new DynValue(args[i]);

            interp.Environment.SupplementLocalLookupTable.Add("args", new DynValue(argsTable));

            var pid = _nextPid++;
            Processes.Add(pid, new Process(pid, string.Join(' ', args), interp) { FilePath = filePath });

            // We don't want to wait for this here.
#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                await ExecuteCode(interp, targetCode);
                Processes.Remove(pid);
            });

            return pid;
        }

        public async Task<DynValue> ExecuteCode(Interpreter interpreter, string code)
        {
            try
            {
                return await interpreter.ExecuteAsync(code);
            }
            catch (ScriptTerminationException)
            {
                // Kernel.Instance.Terminal.WriteLine($"\uFF3FBREAK BY USER\uFF40");
            }
            catch (ScannerException se)
            {
                Kernel.Instance.Terminal.WriteLine(
                    $"\uFF24LEXER ERROR | LINE {se.Line}\uFF40\n{se.Message}\n");
            }
            catch (ParserException pe)
            {
                Kernel.Instance.Terminal.WriteLine(
                    $"\uFF24PARSER ERROR | LINE {(pe.ScannerState?.Line)?.ToString() ?? "??"}\uFF40\n{pe.Message}\n");
            }
            catch (RuntimeException re)
            {
                Kernel.Instance.Terminal.WriteLine(
                    $"\uFF24RUNTIME ERROR | LINE {(re?.Line - 1).ToString() ?? "??"}\uFF40\n{re.Message}\n");
            }
            catch (ClrFunctionException cfe)
            {
                Kernel.Instance.Terminal.WriteLine($"\uFF24BUILT-IN FUNCTION ERROR\uFF40\n{cfe.Message}\n");
            }
            catch (KernelException ke)
            {
                Kernel.Instance.Terminal.WriteLine($"\uFF24KERNEL ERROR\uFF40\n{ke.Message}\n");
            }
            catch (Exception e)
            {
                Kernel.Instance.Terminal.WriteLine($"\uFF24!!! INTERNAL SYSTEM ERROR !!!\uFF40\n");
                Log.Exception(e);
            }
            finally
            {
                if (interpreter.Environment.SupplementLocalLookupTable.ContainsKey("args"))
                    interpreter.Environment.SupplementLocalLookupTable.Remove("args");
            }

            return DynValue.Zero;
        }
        
        private Interpreter CreateProcess()
        {
            var interp = new Interpreter();

            interp.Environment.LoadCoreRuntime();
            interp.Environment.RegisterPackage<SysLibrary>();
            interp.Environment.RegisterPackage<MemLibrary>();
            interp.Environment.RegisterPackage<VgaLibrary>();
            interp.Environment.RegisterPackage<FsLibrary>();

            interp.Memory = Kernel.Instance.Memory;

            return interp;
        }
    }
}