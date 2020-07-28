using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private AstNode ForLoop()
        {
            var line = Match(TokenType.For);
            AstNode assignment;

            if (Scanner.State.CurrentToken.Type == TokenType.LocalVar)
                assignment = Assignment(null, true);
            else
                assignment = Assignment(null, false);

            AstNode step = null;

            Match(TokenType.Comma);
            var targetValue = Comparison();

            if (Scanner.State.CurrentToken.Type == TokenType.Colon)
            {
                Match(TokenType.Colon);
                step = Comparison();
            }

            Match(TokenType.Do);
            var statementList = LoopStatementList();
            Match(TokenType.End);

            return new ForLoopNode(assignment, targetValue, step, statementList) { Line = line };
        }
    }
}
