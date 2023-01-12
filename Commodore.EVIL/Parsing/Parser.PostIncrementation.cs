using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        public AstNode PostIncrementation(string identifier)
        {
            var line = Match(TokenType.Increment);
            return new PostIncrementationNode(Variable(identifier)) { Line = line };
        }
    }
}
