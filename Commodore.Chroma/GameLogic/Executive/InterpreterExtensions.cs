using Commodore.EVIL.Execution;
using Commodore.GameLogic.Core;

namespace Commodore.GameLogic.Executive
{
    public static class InterpreterExtensions
    {
        public static void TerminateWithKernelMessage(this Interpreter interpreter, string message)
        {
            interpreter.BreakExecution = true;
            Kernel.Instance.Terminal.WriteLine($"// \uFF04KERNEL ERROR\uFF40 // {message}");
            interpreter.BreakExecution = false;
        }
    }
}
