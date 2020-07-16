using Chroma.Input;

namespace Commodore.GameLogic.Executive.CodeEditor.Bindings
{
    public class StatusControl : KeyBindings
    {
        public StatusControl(Editor editor) : base(editor)
        {
            editor.Bind(false, false, KeyCode.F1, SetRegularTypingMode);
            editor.Bind(false, false, KeyCode.F2, SetPetsciiTypingMode);
            editor.Bind(false, false, KeyCode.F3, SetShiftedPetsciiTypingMode);

            editor.Bind(false, false, KeyCode.F5, ToggleSyntaxHighlighter);

            editor.Bind(true, false, KeyCode.X, Exit);
            editor.Bind(true, false, KeyCode.Q, ExitDiscard);
        }

        public void ToggleSyntaxHighlighter()
        {
            Editor.TextRenderer.DisableSyntaxHighlighter = !Editor.TextRenderer.DisableSyntaxHighlighter;
        }

        public void SetRegularTypingMode()
        {
            Editor.Options.TypingMode = TypingMode.Regular;
        }

        public void SetPetsciiTypingMode()
        {
            Editor.Options.TypingMode = TypingMode.Petscii;
        }

        public void SetShiftedPetsciiTypingMode()
        {
            Editor.Options.TypingMode = TypingMode.PetsciiShifted;
        }

        private void ExitDiscard()
        {
            Editor.Reset();
            Editor.IsVisible = false;
        }

        private void Exit()
        {
            if (Editor.Buffer.Dirty)
            {
                Editor.ModeLine.Notify("SAVE CHANGES FIRST OR DISCARD WITH CTRL-Q");
                return;
            }

            Editor.Reset();
            Editor.IsVisible = false;
        }
    }
}
