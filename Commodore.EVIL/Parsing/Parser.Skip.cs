using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        public AstNode Skip()
        {
            var line = Match(TokenType.Skip);
            return new SkipNode() { Line = line };
        }
    }
}
