using Commodore.EVIL.Abstraction;
using Commodore.EVIL.AST.Base;
using Commodore.EVIL.AST.Nodes;
using Commodore.EVIL.Diagnostics;
using Commodore.EVIL.Exceptions;
using Commodore.EVIL.Parsing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Commodore.EVIL.Execution
{
    public partial class Interpreter : AstVisitor
    {
        public delegate DynValue ClrFunction(Interpreter interpreter, ClrFunctionArguments args);

        public bool BreakExecution { get; set; }
        public bool SuspendExecution { get; set; }

        public Stack<CallStackItem> CallStack { get; }
        public Stack<LoopStackItem> LoopStack { get; }

        public Environment Environment { get; set; }

        public IMemory Memory { get; set; }
        public Parser Parser { get; }

        public Interpreter()
        {
            CallStack = new Stack<CallStackItem>();
            LoopStack = new Stack<LoopStackItem>();

            Environment = new Environment(this);
            Parser = new Parser();
        }

        public Interpreter(Environment env)
        {
            Environment = env;
        }

        public DynValue Execute(string sourceCode)
        {
            Parser.LoadSource(sourceCode);
            var node = Parser.Parse();

            try
            {
                return Visit(node);
            }
            catch (ExitStatementException)
            {
                CallStack.Clear();
                LoopStack.Clear();

                return DynValue.Zero;
            }
        }

        public async Task<DynValue> ExecuteAsync(string sourceCode)
        {
            Parser.LoadSource(sourceCode);
            var node = Parser.Parse();

            try
            {
                return await Task.Run(async () => await VisitAsync(node));
            }
            catch (ExitStatementException)
            {
                CallStack.Clear();
                LoopStack.Clear();

                return DynValue.Zero;
            }
        }

        public override DynValue Visit(RootNode rootNode)
        {
            return ExecuteStatementList(rootNode.Children).Result;
        }

        public async Task<DynValue> VisitAsync(RootNode rootNode)
        {
            return await ExecuteStatementList(rootNode.Children);
        }

        public List<CallStackItem> StackTrace()
        {
            return new List<CallStackItem>(CallStack);
        }

        private async Task<DynValue> ExecuteStatementList(List<AstNode> statements)
        {
            var retVal = DynValue.Zero;

            if (BreakExecution)
            {
                BreakExecution = false;
                throw new ScriptTerminationException("Execution stopped by user.");
            }

            foreach (var statement in statements)
            {
                while (SuspendExecution)
                {
                    await Task.Delay(1);
                }
                
                if (BreakExecution)
                {
                    BreakExecution = false;
                    throw new ScriptTerminationException("Execution stopped by user.");
                }

                if (LoopStack.Count > 0)
                {
                    var loopStackTop = LoopStack.Peek();

                    if (loopStackTop.BreakLoop || loopStackTop.SkipThisIteration)
                        break;
                }

                if (CallStack.Count > 0)
                {
                    var callStackTop = CallStack.Peek();

                    if (callStackTop.ReturnNow)
                    {
                        retVal = callStackTop.ReturnValue;
                        break;
                    }
                }

                retVal = Visit(statement);
            }

            return retVal;
        }
    }
}