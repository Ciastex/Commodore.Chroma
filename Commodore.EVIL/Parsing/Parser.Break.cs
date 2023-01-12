using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        public AstNode Break()
        {
            var line = Match(TokenType.Break);
            return new BreakNode() { Line = line };
        }
    }
}
