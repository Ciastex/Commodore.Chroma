using Commodore.EVIL.AST.Base;

namespace Commodore.EVIL.AST.Nodes
{
    public class PostDecrementationNode : AstNode
    {
        public VariableNode Variable { get; }

        public PostDecrementationNode(VariableNode variable)
        {
            Variable = variable;
        }
    }
}
