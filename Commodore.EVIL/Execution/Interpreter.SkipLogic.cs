using Commodore.EVIL.Abstraction;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Exceptions;

namespace Commodore.EVIL.Execution
{
    public partial class Interpreter
    {
        public override DynValue Visit(SkipNode skipNode)
        {
            if (LoopStack.Count <= 0)
                throw new RuntimeException("Unexpected 'skip' outside of a loop.", skipNode.Line);

            LoopStack.Peek().SkipThisIteration = true;

            return DynValue.Zero;
        }
    }
}
