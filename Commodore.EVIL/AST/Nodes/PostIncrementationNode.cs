using Commodore.EVIL.AST.Base;

namespace Commodore.EVIL.AST.Nodes
{
    public class PostIncrementationNode : AstNode
    {
        public VariableNode Variable { get; }

        public PostIncrementationNode(VariableNode variable)
        {
            Variable = variable;
        }
    }
}
