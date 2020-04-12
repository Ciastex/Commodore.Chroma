using Commodore.EVIL.Abstraction;
using Commodore.EVIL.AST.Nodes;

namespace Commodore.EVIL.Execution
{
    public partial class Interpreter
    {
        public override DynValue Visit(NumberNode numberNode)
        {
            return numberNode.Value;
        }

        public override DynValue Visit(StringNode stringNode)
        {
            return stringNode.Value;
        }

        public override DynValue Visit(TableNode tableNode)
        {
            return new DynValue(new Table());
        }
    }
}
