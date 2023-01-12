using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Enums;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private AstNode UndefineSymbol()
        {
            var line = Match(TokenType.Undef);

            var node = new UndefNode { Type = UndefineType.Global, Line = line };

            if (Scanner.State.CurrentToken.Type == TokenType.Fn)
            {
                Match(TokenType.Fn);
                node.Type = UndefineType.Function;
            }
            else if (Scanner.State.CurrentToken.Type == TokenType.LocalVar)
            {
                Match(TokenType.LocalVar);
                node.Type = UndefineType.Local;
            }

            node.Name = (string)Scanner.State.CurrentToken.Value;
            Match(TokenType.Identifier);

            return node;
        }
    }
}
