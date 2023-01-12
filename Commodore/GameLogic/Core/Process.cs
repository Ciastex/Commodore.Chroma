using System.Collections.Generic;
using Commodore.EVIL.Execution;

namespace Commodore.GameLogic.Core
{
    public class Process
    {
        public int Pid { get; }
        public string CommandLine { get; }

        public List<string> ImportedLibraryPaths { get; }

        public int? Parent { get; }
        public List<int> Children { get; }

        public Interpreter Interpreter { get; }
        public string FilePath { get; set; }

        public Process(int pid, string commandLine, Interpreter interpreter)
        {
            Pid = pid;
            CommandLine = commandLine;

            ImportedLibraryPaths = new List<string>();
            Interpreter = interpreter;

            Children = new List<int>();
        }

        public Process(int pid, string commandLine, Interpreter interpreter, int? parent)
            : this(pid, commandLine, interpreter)
        {
            Parent = parent;
        }
    }
}