using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private AstNode Assignment(string identifier = null, bool isLocal = false)
        {
            if (isLocal)
                Match(TokenType.LocalVar);

            var variable = Variable(identifier);

            int line = Match(TokenType.Assign);
            var right = Comparison();

            return new AssignmentNode(variable, right, isLocal) { Line = line };
        }
    }
}
