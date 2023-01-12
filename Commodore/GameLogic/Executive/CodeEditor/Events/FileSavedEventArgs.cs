using System;

namespace Commodore.GameLogic.Executive.CodeEditor.Events
{
    public class FileSavedEventArgs : EventArgs
    {
		public string Contents { get; set; }
		public string FilePath { get; set; }
    }
}
