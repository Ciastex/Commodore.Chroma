using Commodore.EVIL.Abstraction.Base;
using Commodore.EVIL.AST.Base;
using System.Collections.Generic;

namespace Commodore.EVIL.Abstraction
{
    public class ScriptFunction : IFunction
    {
        public List<AstNode> StatementList { get; }
        public List<string> ParameterNames { get; }

        public ScriptFunction(List<AstNode> statementList, List<string> parameterNames)
        {
            StatementList = statementList;
            ParameterNames = parameterNames;
        }
    }
}