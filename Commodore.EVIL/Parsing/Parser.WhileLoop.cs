using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private AstNode WhileLoop()
        {
            var line = Match(TokenType.While);
            var expression = Comparison();
            Match(TokenType.Do);
            var statementList = LoopStatementList();
            Match(TokenType.End);

            return new WhileLoopNode(expression, statementList) { Line = line };
        }
    }
}
