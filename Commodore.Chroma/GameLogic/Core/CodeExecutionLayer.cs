using Commodore.Engine;
using Commodore.Engine.FrameworkExtensions;
using Commodore.EVIL.Abstraction;
using Commodore.EVIL.Exceptions;
using Commodore.EVIL.Execution;
using Commodore.GameLogic.Core.Exceptions;
using Commodore.GameLogic.Executive.ComplexRuntime;
using Commodore.GameLogic.Executive.EVILRuntime;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Commodore.GameLogic.Core
{
    public class CodeExecutionLayer
    {
        public bool IsExecuting { get; private set; }
        public Interpreter Interpreter { get; private set; }

        public CodeExecutionLayer() => Reset();

        public void Reset()
        {
            if (Interpreter == null)
            {
                Interpreter = new Interpreter();

                Interpreter.Environment.LoadCoreRuntime();
                Interpreter.Environment.RegisterPackage<SysLibrary>();
                Interpreter.Environment.RegisterPackage<MemLibrary>();
                Interpreter.Environment.RegisterPackage<VgaLibrary>();
                Interpreter.Environment.RegisterPackage<FsLibrary>();
                Interpreter.Environment.RegisterPackage<NetLibrary>();

                Interpreter.Memory = Kernel.Instance.Memory;
            }
            else
            {
                Interpreter.Environment.Functions.Clear();
                Interpreter.Environment.Globals.Clear();
            }
        }

        public async Task ExecuteProgram(string code, params string[] args)
        {
            string targetCode = code ?? string.Empty;

            using (var fs = G.ContentManager.Open("Sources/Templates/program.template.cplx"))
            {
                using (var sr = new StreamReader(fs))
                    targetCode = (await sr.ReadToEndAsync()).Replace("%prepped_source%", targetCode);
            }

            var argsTable = new Table();
            for (var i = 0; i < args.Length; i++)
                argsTable[i] = new DynValue(args[i]);

            Interpreter.Environment.SupplementLocalLookupTable.Add("args", new DynValue(argsTable));
            await ExecuteCode(targetCode);
        }

        public async Task<DynValue> ExecuteCode(string code)
        {
            try
            {
                IsExecuting = true;

                var ret = await Interpreter.ExecuteAsync(code);
                Reset();

                return ret;
            }
            catch (ScannerException se)
            {
                Kernel.Instance.Terminal.WriteLine($"\uFF24LEXER ERROR ({se.Line} : {se.Column})\uFF40\n{se.Message}\n");
            }
            catch (ParserException pe)
            {
                Kernel.Instance.Terminal.WriteLine($"\uFF24PARSER ERROR // LINE {(pe.ScannerState?.Line - 1).ToString() ?? "??"}\uFF40\n{pe.Message}\n");
            }
            catch (RuntimeException re)
            {
                Kernel.Instance.Terminal.WriteLine($"\uFF24RUNTIME ERROR // LINE {re.Line.ToString() ?? "??"}\uFF40\n{re.Message}\n");
            }
            catch (ScriptTerminationException)
            {
                Kernel.Instance.Terminal.WriteLine($"\uFF3FBREAK BY USER\uFF40");
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
                DebugLog.Exception(e);
            }
            finally
            {
                if (Interpreter.Environment.SupplementLocalLookupTable.ContainsKey("args"))
                    Interpreter.Environment.SupplementLocalLookupTable.Remove("args");

                IsExecuting = false;
            }

            return DynValue.Zero;
        }
    }
}
