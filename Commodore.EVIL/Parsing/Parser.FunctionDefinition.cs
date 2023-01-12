using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;
using System.Collections.Generic;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private AstNode FunctionDefinition()
        {
            var line = Match(TokenType.Fn);
            var procName = (string)Scanner.State.CurrentToken.Value;
            Match(TokenType.Identifier);

            Match(TokenType.LParenthesis);
            var parameterList = new List<string>();

            while (Scanner.State.CurrentToken.Type != TokenType.RParenthesis)
            {
                parameterList.Add((string)Scanner.State.CurrentToken.Value);
                Match(TokenType.Identifier);

                if (Scanner.State.CurrentToken.Type == TokenType.RParenthesis)
                    break;

                Match(TokenType.Comma);
            }
            Match(TokenType.RParenthesis);

            var statementList = FunctionStatementList();
            Match(TokenType.End);

            return new FunctionDefinitionNode(procName, statementList, parameterList) { Line = line };
        }
    }
}
