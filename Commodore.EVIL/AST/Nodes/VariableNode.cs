using Commodore.EVIL.AST.Base;

namespace Commodore.EVIL.AST.Nodes
{
    public class VariableNode : AstNode
    {
        public string Name { get; }

        public VariableNode(string name)
        {
            Name = name;
        }
    }
}