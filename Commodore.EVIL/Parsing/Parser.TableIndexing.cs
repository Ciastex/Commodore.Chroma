using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        public AstNode TableIndexing(string identifier)
        {
            var variableNode = Variable(identifier);
            var line = Match(TokenType.LBracket);
            var keyExpression = Comparison();

            Match(TokenType.RBracket);

            return new TableIndexingNode(variableNode, keyExpression) { Line = line };
        }
    }
}
