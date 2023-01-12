using Commodore.EVIL.Abstraction;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Exceptions;

namespace Commodore.EVIL.Execution
{
    public partial class Interpreter
    {
        public override DynValue Visit(ExitNode exitNode)
        {
            throw new ExitStatementException();
        }
    }
}
