using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        public AstNode PostDecrementation(string identifier)
        {
            var line = Match(TokenType.Decrement);
            return new PostDecrementationNode(Variable(identifier)) { Line = line };
        }
    }
}
