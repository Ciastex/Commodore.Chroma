using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        public AstNode TableAssignment(string identifier)
        {
            var variable = Variable(identifier);
            var line = Match(TokenType.LBracket);
            var keyExpression = Comparison();
            Match(TokenType.RBracket);

            Match(TokenType.Assign);

            var valueExpression = Comparison();

            return new TableAssignmentNode(variable, keyExpression, valueExpression) { Line = line };
        }
    }
}
