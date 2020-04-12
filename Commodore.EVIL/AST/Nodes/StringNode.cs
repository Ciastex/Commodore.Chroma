using Commodore.EVIL.Abstraction;
using Commodore.EVIL.AST.Base;

namespace Commodore.EVIL.AST.Nodes
{
    public class StringNode : AstNode
    {
        public DynValue Value { get; }

        public StringNode(string value)
        {
            Value = new DynValue(value);
        }
    }
}