using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Enums;

namespace Commodore.EVIL.AST.Nodes
{
    public class CompoundAssignmentNode : AstNode
    {
        public VariableNode Variable { get; }
        public AstNode Right { get; }
        public CompoundAssignmentType Operation { get; }

        public CompoundAssignmentNode(VariableNode variable, AstNode right, CompoundAssignmentType operation)
        {
            Variable = variable;
            Right = right;
            Operation = operation;
        }
    }
}
