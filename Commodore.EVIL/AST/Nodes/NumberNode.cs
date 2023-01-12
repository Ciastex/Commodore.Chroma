using Commodore.EVIL.Abstraction;
using Commodore.EVIL.AST.Base;

namespace Commodore.EVIL.AST.Nodes
{
    public class NumberNode : AstNode
    {
        public DynValue Value { get; }

        public NumberNode(double number)
        {
            Value = new DynValue(number);
        }
    }
}