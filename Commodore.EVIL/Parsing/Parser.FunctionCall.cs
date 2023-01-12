using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Exceptions;
using Commodore.EVIL.Lexical;
using System.Collections.Generic;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private AstNode FunctionCall(string identifier)
        {
            var functionName = identifier;

            var line = Match(TokenType.LParenthesis);

            var parameters = new List<AstNode>();

            while (Scanner.State.CurrentToken.Type != TokenType.RParenthesis)
            {
                if (Scanner.State.CurrentToken.Type == TokenType.EOF)
                    throw new ParserException($"Unexpected EOF in the function call stated in line {line}.");

                parameters.Add(Expression());

                if (Scanner.State.CurrentToken.Type == TokenType.RParenthesis)
                    break;

                Match(TokenType.Comma);
            }

            Match(TokenType.RParenthesis);

            return new FunctionCallNode(functionName, parameters) { Line = line };
        }
    }
}
