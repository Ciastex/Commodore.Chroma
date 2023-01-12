using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        public AstNode Exit()
        {
            var line = Match(TokenType.Exit);
            return new ExitNode() { Line = line };
        }
    }
}
