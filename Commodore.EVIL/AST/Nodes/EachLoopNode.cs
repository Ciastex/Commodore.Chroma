using Commodore.EVIL.AST.Base;
using System.Collections.Generic;

namespace Commodore.EVIL.AST.Nodes
{
    public class EachLoopNode : AstNode
    {
        public VariableNode KeyNode { get; }
        public VariableNode ValueNode { get; }
        public AstNode TableNode { get; }
        public List<AstNode> StatementList { get; }

        public EachLoopNode(VariableNode keyNode, VariableNode valueNode, AstNode tableNode, List<AstNode> statementList)
        {
            KeyNode = keyNode;
            ValueNode = valueNode;
            TableNode = tableNode;

            StatementList = statementList;
        }
    }
}
