using System.Collections.Generic;
using Commodore.EVIL.Execution;

namespace Commodore.GameLogic.Core
{
    public class Process
    {
        public int Pid { get; }
        public string CommandLine { get; }

        public List<string> ImportedLibraryPaths { get; }

        public Interpreter Interpreter { get; }
        
        public string FilePath { get; set; }

        public Process(int pid, string commandLine, Interpreter interpreter)
        {
            Pid = pid;
            CommandLine = commandLine;

            ImportedLibraryPaths = new List<string>();
            Interpreter = interpreter;
        }
    }
}