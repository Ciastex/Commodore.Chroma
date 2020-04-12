using Commodore.Engine.FrameworkExtensions;
using Commodore.GameLogic.Core.IO.Mappings;
using Commodore.GameLogic.Display;
using Chroma.Input;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Commodore.GameLogic.Core.IO
{
    public class Terminal
    {
        private bool _awaitingInputString;
        private bool _awaitingInputCharacter;

        private int _historyIndex;
        private int _inputBufferIndex;
        private string _inputBuffer;
        private int _keyBuffer;

        private readonly VGA _vga;

        public List<string> InputHistory { get; }

        public Terminal(VGA vga)
        {
            _vga = vga;
            _inputBuffer = string.Empty;

            InputHistory = new List<string>();
        }

        public void ResetInputHistory()
        {
            InputHistory.Clear();
            _historyIndex = 0;
        }

        public void Update(float deltaTime)
        {
            _vga.Cursor.ForceHidden = !_awaitingInputString && !_awaitingInputCharacter;
        }

        public async Task<string> ReadLine(string prompt, CancellationToken token)
        {
            Write(prompt);
            _awaitingInputString = true;

            while (_awaitingInputString)
            {
                if (token != null && token.IsCancellationRequested)
                {
                    _inputBuffer = string.Empty;
                    _inputBufferIndex = 0;
                    _awaitingInputString = false;
                }
                else
                    await Task.Delay(1);
            }

            if (!string.IsNullOrWhiteSpace(_inputBuffer))
            {
                InputHistory.Add(_inputBuffer);
                _historyIndex = InputHistory.Count;
            }
            var output = _inputBuffer;
            _inputBuffer = string.Empty;
            _inputBufferIndex = 0;

            return output;
        }

        public async Task<int> Read(string prompt, CancellationToken token)
        {
            Write(prompt);
            _awaitingInputCharacter = true;

            while (_awaitingInputCharacter)
            {
                if (token != null && token.IsCancellationRequested)
                {
                    _keyBuffer = 0;
                    _awaitingInputString = false;
                }
                else
                    await Task.Delay(1);
            }

            var output = _keyBuffer;
            _keyBuffer = 0;

            return output;
        }

        public void Write(char character)
        {
            if (ColorMappings.IsForegroundMapping(character))
            {
                _vga.ActiveForegroundColor = ColorMappings.GetForeground(character);
                return;
            }

            if (ColorMappings.IsBackgroundMapping(character))
            {
                _vga.ActiveBackgroundColor = ColorMappings.GetBackground(character);
                return;
            }

            if (character == '\uff40')
            {
                _vga.ActiveForegroundColor = _vga.DefaultForegroundColor;
                return;
            }

            if(character == '\ufe40')
            {
                _vga.ActiveBackgroundColor = _vga.DefaultBackgroundColor;
                return;
            }

            if (character == '\b')
            {
                MoveCursorBackwards();
                _vga.PutCharAt(' ', _vga.CursorX, _vga.CursorY);
            }
            else if (character == '\n')
            {
                _vga.CursorX = 0;
                _vga.CursorY++;

                if (_vga.CursorY >= _vga.TotalRows)
                {
                    _vga.CursorY = (ushort)(_vga.TotalRows - 1);
                    _vga.ScrollUp();
                }
            }
            else if (character == '\r')
            {
                _vga.CursorX = 0;
            }
            else
            {
                _vga.PutCharAt(character, _vga.CursorX, _vga.CursorY);
                MoveCursorForwards();
            }
        }

        public void Write(string output)
        {
            if (string.IsNullOrEmpty(output)) return;

            foreach (var c in output)
                Write(c);
        }

        public void WriteLine(string output)
        {
            Write(output);
            Write('\n');
        }

        public void WriteLine()
        {
            Write('\n');
        }

        public async Task WriteTyped(string output, int charDelay = 15)
        {
            foreach (var c in output)
            {
                Write(c);
                await Task.Delay(charDelay);
            }
        }

        public async Task WriteTypedLine(string output, int charDelay = 15)
        {
            await WriteTyped(output, charDelay);
            Write('\n');
        }

        private void HandleReturn()
        {
            if (_awaitingInputString)
                _awaitingInputString = false;

            Write('\n');
        }

        private void HandleBackspace()
        {
            if (_awaitingInputString)
            {
                if (string.IsNullOrEmpty(_inputBuffer) || _inputBufferIndex == 0) return;

                var split = _inputBuffer.SplitAt(_inputBufferIndex);
                split[0] = split[0].Substring(0, split[0].Length - 1);

                if (_inputBufferIndex != _inputBuffer.Length)
                    split[0] += " ";

                _inputBuffer = string.Join("", split);
                _inputBufferIndex--;

                Write('\b');
            }
            else
            {
                Write('\b');
            }
        }

        private void HandlePrintableCharacter(char c)
        {
            if (_awaitingInputString)
            {
                if (_inputBufferIndex == _inputBuffer.Length)
                {
                    _inputBuffer += c;
                    _inputBufferIndex = _inputBuffer.Length;
                }
                else
                {
                    var split = _inputBuffer.SplitAt(_inputBufferIndex);
                    split[1] = c + split[1].Substring(1);

                    _inputBuffer = string.Join("", split);
                    _inputBufferIndex++;
                }
            }

            Write(c);
        }

        private void MoveCursorBackwards()
        {
            _vga.CursorX--;

            if (_vga.CursorX < 0)
            {
                if (_vga.CursorY > 0)
                {
                    _vga.CursorX = (ushort)(_vga.TotalColumns - 1);
                    _vga.CursorY--;
                }
                else
                {
                    _vga.CursorX = 0;
                }
            }
        }

        private void MoveCursorForwards()
        {
            _vga.CursorX++;

            if (_vga.CursorX >= _vga.TotalColumns)
            {
                _vga.CursorX = 0;
                _vga.CursorY++;

                if (_vga.CursorY >= _vga.TotalRows)
                {
                    _vga.CursorY = (ushort)(_vga.TotalRows - 1);
                    _vga.ScrollUp();
                }
            }
        }

        public void KeyPressed(KeyCode keyCode, KeyModifiers modifiers)
        {
            if (!_awaitingInputCharacter && !_awaitingInputString)
                return;

            if (_awaitingInputCharacter)
            {
                _keyBuffer = (int)keyCode;
                _awaitingInputCharacter = false;

                return;
            }

            if (_awaitingInputString)
            {
                if (modifiers.HasFlag(KeyModifiers.LeftControl) ||
                    modifiers.HasFlag(KeyModifiers.RightControl))
                {
                    var shiftPressed = modifiers.HasFlag(KeyModifiers.LeftShift) ||
                                       modifiers.HasFlag(KeyModifiers.RightShift);
                    
                    var dict = shiftPressed ? PetsciiControlCodes.PetsciiSymbolMappings 
                                            : PetsciiControlCodes.PetsciiSymbolMappingShifted;

                    if (dict.ContainsKey(keyCode))
                    {
                        var petscii = dict[keyCode];
                        var measure = _vga.MeasureString(petscii.ToString());

                        if (measure.X != 0)
                            HandlePrintableCharacter(petscii);
                    }
                }
                else if (keyCode == KeyCode.Left)
                {
                    if (_inputBufferIndex > 0)
                    {
                        MoveCursorBackwards();
                        _inputBufferIndex--;
                    }
                }
                else if (keyCode == KeyCode.Right)
                {
                    if (_inputBufferIndex + 1 <= _inputBuffer.Length)
                    {
                        MoveCursorForwards();
                        _inputBufferIndex++;
                    }
                }
                else if (keyCode == KeyCode.Up)
                {
                    if (_historyIndex - 1 < 0)
                        return;

                    for (var i = _inputBufferIndex; i < _inputBuffer.Length; i++)
                        MoveCursorForwards();

                    for (var i = 0; i < _inputBuffer.Length; i++)
                        Write('\b');

                    var historyValue = InputHistory[--_historyIndex];
                    Write(historyValue);

                    _inputBuffer = historyValue;
                    _inputBufferIndex = _inputBuffer.Length;
                }
                else if (keyCode == KeyCode.Down)
                {
                    if (_historyIndex + 1 >= InputHistory.Count)
                        return;

                    for (var i = _inputBufferIndex; i < _inputBuffer.Length; i++)
                        MoveCursorForwards();

                    for (var i = 0; i < _inputBuffer.Length; i++)
                        Write('\b');

                    var historyValue = InputHistory[++_historyIndex];

                    Write(historyValue);

                    _inputBuffer = historyValue;
                    _inputBufferIndex = _inputBuffer.Length;
                }
                else if (keyCode == KeyCode.Home)
                {
                    for (var i = 0; i < _inputBufferIndex; i++)
                        MoveCursorBackwards();

                    _inputBufferIndex = 0;
                }
                else if (keyCode == KeyCode.End)
                {
                    for (var i = _inputBufferIndex; i < _inputBuffer.Length; i++)
                        MoveCursorForwards();

                    _inputBufferIndex = _inputBuffer.Length;
                }
                else if (keyCode == KeyCode.Return)
                {
                    HandleReturn();
                }
                else if (keyCode == KeyCode.Backspace)
                {
                    HandleBackspace();
                }
            }
        }

        public void TextInput(char character)
        {
            if (!_awaitingInputString)
                return;

            if (!char.IsControl(character))
            {
                if(_vga.Font.HasGlyph(character))
                    HandlePrintableCharacter(character);
            }
        }
    }
}
