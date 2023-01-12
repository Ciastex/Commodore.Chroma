using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Enums;

namespace Commodore.EVIL.AST.Nodes
{
    public class UndefNode : AstNode
    {
        public string Name { get; set; }
        public UndefineType Type { get; set; }
    }
}
