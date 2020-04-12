using Commodore.EVIL.AST.Base;

namespace Commodore.EVIL.AST.Nodes
{
    public class TableAssignmentNode : AstNode
    {
        public AstNode Variable { get; }
        public AstNode KeyExpression { get; }
        public AstNode ValueExpression { get; }

        public TableAssignmentNode(AstNode variable, AstNode keyExpression, AstNode valueExpression)
        {
            Variable = variable;

            KeyExpression = keyExpression;
            ValueExpression = valueExpression;
        }
    }
}
