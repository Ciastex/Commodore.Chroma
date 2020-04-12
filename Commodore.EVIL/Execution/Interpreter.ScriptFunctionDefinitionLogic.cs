using Commodore.EVIL.Abstraction;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Exceptions;

namespace Commodore.EVIL.Execution
{
    public partial class Interpreter
    {
        public override DynValue Visit(FunctionDefinitionNode scriptFunctionDefinitionNode)
        {
            var name = scriptFunctionDefinitionNode.Name;

            if (Environment.BuiltIns.ContainsKey(name))
            {
                throw new RuntimeException($"Function '{name}' is a built-in function and cannot be redefined.", scriptFunctionDefinitionNode.Line);
            }

            // RegisterFunction allows redefinition
            Environment.RegisterFunction(
                name,
                new ScriptFunction(
                    scriptFunctionDefinitionNode.StatementList,
                    scriptFunctionDefinitionNode.ParameterNames
                )
            );

            return DynValue.Zero;
        }
    }
}
