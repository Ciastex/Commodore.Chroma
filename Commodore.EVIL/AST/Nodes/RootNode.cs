using Commodore.EVIL.AST.Base;
using System.Collections.Generic;

namespace Commodore.EVIL.AST.Nodes
{
    public class RootNode : AstNode
    {
        public List<AstNode> Children { get; }

        public RootNode()
        {
            Children = new List<AstNode>();
        }
    }
}