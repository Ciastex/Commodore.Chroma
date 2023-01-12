using Commodore.EVIL.Abstraction;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Exceptions;

namespace Commodore.EVIL.Execution
{
    public partial class Interpreter
    {
        public override DynValue Visit(ReturnNode returnNode)
        {
            if (CallStack.Count <= 0)
                throw new RuntimeException("Return statement outside of a function.", returnNode.Line);

            var stackTop = CallStack.Peek();
            stackTop.ReturnNow = true;
            stackTop.ReturnValue = Visit(returnNode.Expression);

            return stackTop.ReturnValue;
        }
    }
}
