using Chroma.ContentManagement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Commodore.Engine.FrameworkExtensions;

namespace Commodore.GameLogic.Core.BootSequence
{
    public class BootSequencePlayer
    {
        private IContentProvider ContentManager { get; }

        private bool ScrollUpAfterwards { get; set; }
        private List<SequenceLine> Lines { get; set; }

        public BootSequencePlayer(IContentProvider contentManager)
        {
            ContentManager = contentManager;
        }

        public async Task TryPerformSequence()
        {
            foreach (var line in Lines)
            {
                if (line.Typed)
                {
                    await Kernel.Instance.Terminal.WriteTyped(line.EvaluatedText, line.CharDelay);
                }
                else
                {
                    Kernel.Instance.Terminal.Write(line.EvaluatedText);
                }

                if (!line.NoNewLine)
                {
                    Kernel.Instance.Terminal.Write("\n");
                }

                await Task.Delay(line.LineDelay);
            }

            if (ScrollUpAfterwards)
            {
                for (var i = 0; i < Lines.Count; i++)
                {
                    Kernel.Instance.Vga.ScrollUp();
                    await Task.Delay(1);
                }

                Kernel.Instance.Vga.CursorX = 0;
                Kernel.Instance.Vga.CursorY = 0;
            }
        }

        public void Build(string bootSequenceFileName)
        {
            Lines = new List<SequenceLine>();

            try
            {
                var str = Encoding.UTF8.GetString(ContentManager.Read($"Text/Boot/{bootSequenceFileName}.xml"));
                var xDocument = XDocument.Parse(str);

                var bootSequenceElements = xDocument.Root.Elements("Line");
                var scrollUpElement = xDocument.Root.Element("ScrollUp");

                foreach (var element in bootSequenceElements)
                {
                    var line = new SequenceLine();

                    if (element.HasAttribute("Delay"))
                    {
                        line.LineDelay = int.Parse(element.Attribute("Delay").Value);
                    }

                    if (element.HasAttribute("CharDelay"))
                    {
                        line.CharDelay = int.Parse(element.Attribute("CharDelay").Value);
                    }

                    if (element.HasAttribute("Typed"))
                    {
                        line.Typed = bool.Parse(element.Attribute("Typed").Value);
                    }

                    if (element.HasAttribute("NoNewLine"))
                    {
                        line.NoNewLine = bool.Parse(element.Attribute("NoNewLine").Value);
                    }

                    if (!string.IsNullOrEmpty(element.Value))
                    {
                        line.Text = element.Value;
                    }

                    Lines.Add(line);
                }

                if (scrollUpElement != null)
                {
                    ScrollUpAfterwards = true;
                }
                else
                {
                    ScrollUpAfterwards = false;
                }
            }
            catch (Exception e)
            {
                Kernel.Instance.Terminal.WriteLine("ACTUAL GAME ERROR");
                Kernel.Instance.Terminal.WriteLine("Boot sequence failed to build.\nIf you didn't mess with the XMLs contact the retarded developer.");
                Kernel.Instance.Terminal.WriteLine($"\uFF04Tell him this happened when reading {bootSequenceFileName}\n: {e.Message}.\uFF40");
                Kernel.Instance.Terminal.WriteLine("Feel free to play the game otherwise.\nThis error will stay here every reboot as long as the above thing isn't fixed.");
            }
        }
    }
}
