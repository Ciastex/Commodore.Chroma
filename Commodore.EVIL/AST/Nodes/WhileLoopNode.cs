using Commodore.EVIL.AST.Base;
using System.Collections.Generic;

namespace Commodore.EVIL.AST.Nodes
{
    public class WhileLoopNode : AstNode
    {
        public AstNode Expression { get; }
        public List<AstNode> StatementList { get; }

        public WhileLoopNode(AstNode expression, List<AstNode> statementList)
        {
            Expression = expression;
            StatementList = statementList;
        }
    }
}
