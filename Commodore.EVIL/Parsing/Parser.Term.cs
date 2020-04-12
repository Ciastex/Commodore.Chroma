using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Enums;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Lexical;
using System.Linq;

namespace Commodore.EVIL.Parsing
{
    public partial class Parser
    {
        private static readonly TokenType[] _termOperators = new[]
{
            TokenType.Multiply,
            TokenType.Divide,
            TokenType.Nand,
            TokenType.Modulo,
            TokenType.CompareLessThan,
            TokenType.CompareGreaterThan,
            TokenType.CompareLessOrEqualTo,
            TokenType.CompareGreaterOrEqualTo,
            TokenType.CompareEqual,
            TokenType.CompareNotEqual
        };

        private AstNode Term()
        {
            var node = Factor();
            var token = Scanner.State.CurrentToken;

            while (_termOperators.Contains(token.Type))
            {
                token = Scanner.State.CurrentToken;

                if (token.Type == TokenType.Multiply)
                {
                    var line = Match(TokenType.Multiply);
                    node = new BinaryOperationNode(node, BinaryOperationType.Multiply, Factor()) { Line = line };
                }
                else if (token.Type == TokenType.Divide)
                {
                    var line = Match(TokenType.Divide);
                    node = new BinaryOperationNode(node, BinaryOperationType.Divide, Factor()) { Line = line };
                }
                else if (token.Type == TokenType.Nand)
                {
                    var line = Match(TokenType.Nand);
                    node = new BinaryOperationNode(node, BinaryOperationType.Nand, Factor()) { Line = line };
                }
                else if (token.Type == TokenType.Modulo)
                {
                    var line = Match(TokenType.Modulo);
                    node = new BinaryOperationNode(node, BinaryOperationType.Modulo, Factor()) { Line = line };
                }
                else if (token.Type == TokenType.CompareLessThan)
                {
                    var line = Match(TokenType.CompareLessThan);
                    node = new BinaryOperationNode(node, BinaryOperationType.Less, Factor()) { Line = line };
                }
                else if (token.Type == TokenType.CompareGreaterThan)
                {
                    var line = Match(TokenType.CompareGreaterThan);
                    node = new BinaryOperationNode(node, BinaryOperationType.Greater, Factor()) { Line = line };
                }
                else if (token.Type == TokenType.CompareLessOrEqualTo)
                {
                    var line = Match(TokenType.CompareLessOrEqualTo);
                    node = new BinaryOperationNode(node, BinaryOperationType.LessOrEqual, Factor()) { Line = line };
                }
                else if (token.Type == TokenType.CompareGreaterOrEqualTo)
                {
                    var line = Match(TokenType.CompareGreaterOrEqualTo);
                    node = new BinaryOperationNode(node, BinaryOperationType.GreaterOrEqual, Factor()) { Line = line };
                }
                else if (token.Type == TokenType.CompareEqual)
                {
                    var line = Match(TokenType.CompareEqual);
                    node = new BinaryOperationNode(node, BinaryOperationType.Equal, Factor()) { Line = line };
                }
                else if (token.Type == TokenType.CompareNotEqual)
                {
                    var line = Match(TokenType.CompareNotEqual);
                    node = new BinaryOperationNode(node, BinaryOperationType.NotEqual, Factor()) { Line = line };
                }
            }
            return node;
        }
    }
}
