using Chroma.Input;
using System;
using System.Text;

namespace Commodore.GameLogic.Executive.CodeEditor
{
    public class Hotkey
    {
        public bool Enabled { get; }

        public bool Control { get; }
        public bool Shift { get; }
        public bool Alt { get; }

        public int Precedence
        {
            get
            {
                var precedence = 1;

                if (Control) precedence++;
                if (Shift) precedence++;

                return precedence;
            }
        }

        public KeyCode Key { get; }

        public Action Action { get; }

        public string Descriptor
        {
            get
            {
                var sb = new StringBuilder();

                if (Control)
                    sb.Append("ctrl-");
                if (Shift)
                    sb.Append("shift-");
                if (Alt)
                    sb.Append("alt-");

                sb.Append(Key.ToString().ToLower());

                return sb.ToString();
            }
        }

        public Hotkey(bool control, bool shift, KeyCode key, Action action)
        {
            Control = control;
            Shift = shift;

            Key = key;
            Enabled = true;
            Action = action;
        }

        public bool IsDown()
        {
            if (!Enabled)
                return false;

            if (!(Keyboard.IsKeyDown(KeyCode.LeftControl) || Keyboard.IsKeyDown(KeyCode.RightControl)) && Control)
                return false;

            if (!(Keyboard.IsKeyDown(KeyCode.LeftShift) || Keyboard.IsKeyDown(KeyCode.RightShift)) && Shift)
                return false;

            if (!(Keyboard.IsKeyDown(KeyCode.LeftAlt) || Keyboard.IsKeyDown(KeyCode.RightAlt)) && Alt)
                return false;

            return Keyboard.IsKeyDown(Key);
        }
    }
}
