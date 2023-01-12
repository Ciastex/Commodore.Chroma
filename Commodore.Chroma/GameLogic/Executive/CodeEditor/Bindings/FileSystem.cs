using Commodore.GameLogic.Core.IO.Storage;
using Chroma.Input;
using System;
using System.Text;

namespace Commodore.GameLogic.Executive.CodeEditor.Bindings
{
    public class FileSystem : KeyBindings
    {
        public FileSystem(Editor editor) : base(editor)
        {
            editor.Bind(true, false, KeyCode.O, Save);
        }

        public void Save()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < Editor.Buffer.Lines.Count; i++)
            {
                if (i == Editor.Buffer.CurrentLineNumber)
                    if (Editor.Buffer.CursorX > Editor.Buffer.Lines[i].Length)
                        Editor.Buffer.CursorX = Editor.Buffer.Lines[i].Length;

                if (i < Editor.Buffer.Lines.Count - 1)
                    sb.Append(Editor.Buffer.Lines[i] + "\n");
                else sb.Append(Editor.Buffer.Lines[i]);
            }

            if (Editor.Buffer.CurrentFileName == null)
            {
                void inputCanceled(object sender, EventArgs e)
                {
                    Editor.ModeLine.InputReceived -= inputReceived;
                    Editor.ModeLine.InputCanceled -= inputCanceled;
                }

                void inputReceived(object sender, string input)
                {
                    Editor.ModeLine.InputReceived -= inputReceived;
                    Editor.ModeLine.InputCanceled -= inputCanceled;

                    try
                    {
                        var file = File.Create(input, true);

                        Editor.Buffer.CurrentFileName = input;
                        Editor.OnFileSaved(sb.ToString(), Editor.Buffer.CurrentFileName);

                        Editor.Buffer.Dirty = false;
                    }
                    catch
                    {
                        Editor.ModeLine.Notify("FAILED SAVING THE FILE. INVALID PATH?");
                        return;
                    }
                }

                Editor.ModeLine.InputReceived += inputReceived;
                Editor.ModeLine.InputCanceled += inputCanceled;

                Editor.ModeLine.Read("FILE PATH? ");
            }
            else
            {
                Editor.OnFileSaved(sb.ToString(), Editor.Buffer.CurrentFileName);
                Editor.Buffer.Dirty = false;
            }
        }
    }
}
