namespace Commodore.GameLogic.Core.BootSequence
{
    public class SequenceLine
    {
        public int LineDelay { get; set; } = 180;
        public int CharDelay { get; set; } = 15;

        public bool Typed { get; set; } = false;
        public bool NoNewLine { get; set; } = false;

        public string Text { get; set; } = string.Empty;
        public string EvaluatedText => EvaluateMacros();

        private string EvaluateMacros()
        {
            return Text;
        }
    }
}
