using Commodore.EVIL.Abstraction;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Exceptions;

namespace Commodore.EVIL.Execution
{
    public partial class Interpreter
    {
        public override DynValue Visit(BreakNode breakNode)
        {
            if (LoopStack.Count <= 0)
                throw new RuntimeException("Unexpected 'break' outside a loop.", breakNode.Line);

            LoopStack.Peek().BreakLoop = true;

            return DynValue.Zero;
        }

    }
}
