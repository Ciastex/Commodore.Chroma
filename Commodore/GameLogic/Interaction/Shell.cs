using Commodore.GameLogic.Core;
using Commodore.GameLogic.Core.IO.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Commodore.GameLogic.Interaction
{
    public class Shell
    {
        public delegate bool Command(string[] args);

        public Dictionary<string, Command> BuiltIns { get; }

        public Shell()
        {
            BuiltIns = new Dictionary<string, Command>();

            BuiltIns.Add("edit", (args) =>
            {
                if (args.Length == 0)
                {
                    Kernel.Instance.Editor.Reset();
                    Kernel.Instance.Editor.IsVisible = true;
                }

                if (args.Length > 0)
                    Kernel.Instance.Editor.OpenPath(args[0]);

                return true;
            });
        }

        public async Task<bool> HandleCommand(string input, CancellationToken token)
        {
            try
            {
                var tokens = Tokenize(input);

                var command = tokens[0];
                var argList = tokens.Skip(1).ToList();

                var waitForProcess = true;
                var lastArg = argList.LastOrDefault();

                if (lastArg == "&")
                {
                    waitForProcess = false;
                    argList.RemoveAt(argList.Count - 1);
                }

                var args = argList.ToArray();

                if (BuiltIns.ContainsKey(command))
                    return BuiltIns[command](args);

                var filePath = $"/bin/{command}";

                if (command.StartsWith("./") || command.StartsWith("../") || command.StartsWith("/"))
                {
                    filePath = command;
                }

                if (Directory.Exists(filePath, true))
                {
                    Kernel.Instance.Terminal.WriteLine($"{filePath}: is a directory");
                    return false;
                }

                if (File.Exists(filePath, true))
                {
                    var file = File.Get(filePath, true);

                    if ((file.Attributes & FileAttributes.Executable) != 0)
                    {
                        var pid = Kernel.Instance.ProcessManager.ExecuteProgram(
                            file.GetData(),
                            filePath,
                            Kernel.Instance.InteractionCancellation.Token,
                            args
                        );

                        if (waitForProcess)
                        {
                            await Kernel.Instance.ProcessManager.WaitForProgram(pid, token);
                        }

                        return true;
                    }
                    else
                    {
                        Kernel.Instance.Terminal.WriteLine($"CANNOT EXEC \uFF26{filePath}\uFF40: not an executable");
                        return false;
                    }
                }

                Kernel.Instance.Terminal.WriteLine($"\uFF26UNRECOGNIZED COMMAND AND NO EXECUTABLE FOUND\uFF40\n");
                return false;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Kernel.Instance.Terminal.WriteLine($"\uFF24ERROR:\n\uFF40{e.Message}\n");
            }

            return false;
        }

        private List<string> Tokenize(string input)
        {
            var ret = new List<string>();
            var currentToken = string.Empty;

            for (var i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '\\':
                    {
                        if (i + 1 >= input.Length)
                        {
                            currentToken += '\\';
                        }
                        else
                        {
                            i++;
                            currentToken += input[i];
                        }

                        break;
                    }

                    case '"':
                        if (i + 1 >= input.Length)
                        {
                            currentToken += '"';
                        }
                        else
                        {
                            i++;

                            while (input[i] != '"')
                            {
                                if (i + 1 >= input.Length)
                                    throw new Exception("Unterminated string.");

                                if (input[i] == '\\')
                                {
                                    if (i + 1 >= input.Length)
                                        throw new Exception("Unterminated string.");

                                    i++;
                                    switch (input[i])
                                    {
                                        case '\\':
                                            currentToken += '\\';
                                            i++;
                                            break;

                                        case 'n':
                                            currentToken += '\n';
                                            i++;
                                            break;

                                        case 'r':
                                            currentToken += '\r';
                                            i++;
                                            break;

                                        case '"':
                                            currentToken += '"';
                                            i++;
                                            break;
                                        default: throw new Exception("Unrecognized escape sequence.");
                                    }
                                }
                                else
                                {
                                    currentToken += input[i++];
                                }
                            }
                        }

                        break;

                    case ' ':
                        if (!string.IsNullOrWhiteSpace(currentToken))
                            ret.Add(currentToken);

                        currentToken = string.Empty;
                        break;

                    default:
                        currentToken += input[i];
                        break;
                }
            }

            if (currentToken.Length > 0)
                ret.Add(currentToken);

            return ret;
        }
    }
}