using System;
using System.IO;

namespace Commodore.Engine
{
    public static class DebugLog
    {
        private static StreamWriter LogStreamWriter { get; }

        public static bool ForwardToConsole { get; set; }

        static DebugLog()
        {
            LogStreamWriter = new StreamWriter("debug.log") { AutoFlush = true };
        }

        public static void Info(string message, string context = "")
        {
            var output = $"[::] {context} :: {message}";

            LogStreamWriter.WriteLine(output);

            if (ForwardToConsole)
                Console.WriteLine(output);
        }

        public static void Error(string message, string context = "")
        {
            var output = $"[EE] {context} :: {message}";

            LogStreamWriter.WriteLine(output);

            if (ForwardToConsole)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(output);

                Console.ResetColor();
            }
        }

        public static void Warning(string message, string context = "")
        {
            var output = $"[**] {context} :: {message}";

            LogStreamWriter.WriteLine(output);

            if (ForwardToConsole)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(output);

                Console.ResetColor();
            }
        }

        public static void Exception(Exception e, string context = "")
        {
            var output = $"[!!] {context} :: {e.Message}\n{e.StackTrace}";

            LogStreamWriter.WriteLine(output);

            if (ForwardToConsole)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(output);

                Console.ResetColor();
            }
        }
    }
}
