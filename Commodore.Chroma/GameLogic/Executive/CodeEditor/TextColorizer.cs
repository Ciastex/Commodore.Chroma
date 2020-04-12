using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public class TextColorizer
    {
        private readonly List<string> Keywords = new List<string>
        {
            "fn", "while", "if", "do",
            "else", "end", "for", "skip",
            "break", "ret", "local", "and",
            "or", "undef", "exit", "elif",
            "each"
        };

        private readonly List<string> ValueWords = new List<string>
        {
            "true", "false"
        };

        private readonly List<char> Operators = "()[].,=#$@+-*/%:<>{}?!".ToCharArray().ToList();

        public List<ColorizedSegment> Colorize(string line)
        {
            //todo: clean this code up

            var ret = new List<ColorizedSegment>();

            var currentSegment = new ColorizedSegment();

            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    ret.Add(currentSegment);
                    currentSegment = new ColorizedSegment
                    {
                        Color = EditorColors.String
                    };

                    currentSegment.Value += line[i++];

                    i += GetEnclosedString(line, out string token, i);

                    currentSegment.Value += token;

                    ret.Add(currentSegment);
                    currentSegment = new ColorizedSegment();
                }
                else if (line[i] == '/' && i + 1 < line.Length && line[i + 1] == '/')
                {
                    ColorizeSegment(currentSegment);
                    ret.Add(currentSegment);
                    currentSegment = new ColorizedSegment
                    {
                        Color = EditorColors.Comment,
                    };

                    currentSegment.Value += line[i++];
                    currentSegment.Value += line[i++];

                    while (i < line.Length)
                        currentSegment.Value += line[i++];

                    break;
                }
                else if (line[i] == ' ')
                {
                    ColorizeSegment(currentSegment);
                    currentSegment.Value += " ";

                    ret.Add(currentSegment);
                    currentSegment = new ColorizedSegment();
                }
                else if (Operators.Contains(line[i]))
                {
                    ColorizeSegment(currentSegment);
                    ret.Add(currentSegment);

                    currentSegment = new ColorizedSegment { Color = EditorColors.Operator };
                    currentSegment.Value += line[i];
                    ret.Add(currentSegment);

                    currentSegment = new ColorizedSegment();
                }
                else
                {
                    currentSegment.Value += line[i];
                }
            }

            if (!string.IsNullOrEmpty(currentSegment.Value))
            {
                ColorizeSegment(currentSegment);
                ret.Add(currentSegment);
            }

            return ret;
        }

        private void ColorizeSegment(ColorizedSegment currentSegment)
        {
            if (Keywords.Contains(currentSegment.Value.Trim()))
                currentSegment.Color = EditorColors.Keyword;
            else if (ValueWords.Contains(currentSegment.Value.Trim()))
                currentSegment.Color = EditorColors.ValueWord;
            else if (int.TryParse(currentSegment.Value, out int _))
                currentSegment.Color = EditorColors.Number;
            else if (IsHexWithPrefix(currentSegment.Value))
                currentSegment.Color = EditorColors.Number;
        }

        private int GetEnclosedString(string str, out string token, int index)
        {
            var ret = string.Empty;

            int i;
            for (i = index; i < str.Length; i++)
            {
                if (str[i] == '\\')
                {
                    ret += '\\';

                    if (i + 1 > str.Length)
                        break;

                    if (i + 1 < str.Length)
                    {
                        i++;
                        ret += str[i];
                    }
                    continue;
                }

                ret += str[i];
                if (str[i] == '"') break;
            }

            token = ret;
            return i - index;
        }

        private bool IsHexWithPrefix(string value)
        {
            if (!value.StartsWith("0x"))
                return false;

            return int.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out int _);
        }
    }
}
