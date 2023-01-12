namespace Commodore.GameLogic.Executive.CodeEditor.Bindings
{
    public abstract class KeyBindings
    {
        protected Editor Editor { get; private set; }

        public KeyBindings(Editor editor)
        {
            Editor = editor;
        }
    }
}
