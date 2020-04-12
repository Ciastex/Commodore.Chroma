using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private AstNode Return()
        {
            var line = Match(TokenType.Ret);
            return new ReturnNode(Expression()) { Line = line };
        }
    }
}
