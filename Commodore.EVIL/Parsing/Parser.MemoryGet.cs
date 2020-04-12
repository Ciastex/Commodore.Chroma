using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private MemoryGetNode MemoryGet()
        {
            var line = Match(TokenType.LBracket);

            var size = 0;

            if (Scanner.State.CurrentToken.Type == TokenType.Colon)
            {
                Match(TokenType.Colon);
                size++;
            }

            var node = Expression();

            if (Scanner.State.CurrentToken.Type == TokenType.Colon)
            {
                Match(TokenType.Colon);
                size++;
            }

            Match(TokenType.RBracket);
            return new MemoryGetNode(node, (MemoryGetNode.OperandSize)size) { Line = line };
        }
    }
}
