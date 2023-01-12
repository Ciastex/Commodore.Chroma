using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Exceptions;
using Commodore.EVIL.Lexical;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private AstNode IfCondition()
        {
            var line = Match(TokenType.If);
            Match(TokenType.LParenthesis);

            var expression = Expression();

            Match(TokenType.RParenthesis);
            Match(TokenType.Do);

            var node = new ConditionNode { Line = line };
            node.IfElifBranches.Add(expression, ConditionStatementList());

            while (Scanner.State.CurrentToken.Type == TokenType.Elif || Scanner.State.CurrentToken.Type == TokenType.Else)
            {
                if (Scanner.State.CurrentToken.Type == TokenType.Elif)
                {
                    Match(TokenType.Elif);
                    Match(TokenType.LParenthesis);

                    expression = Expression();

                    Match(TokenType.RParenthesis);
                    Match(TokenType.Do);

                    node.IfElifBranches.Add(expression, ConditionStatementList());
                }
                else if (Scanner.State.CurrentToken.Type == TokenType.Else)
                {
                    Match(TokenType.Else);
                    node.ElseBranch = ConditionStatementList();
                    Match(TokenType.End);

                    return node;
                }
                else throw new ParserException($"Expected 'end' or 'else' or 'elif', got {Scanner.State.CurrentToken.Type}", Scanner.State);
            }

            Match(TokenType.End);
            return node;
        }
    }
}
