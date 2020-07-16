using Commodore.GameLogic.Persistence;
using System.Linq;

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
            var ret = Text;

            for(var i = 1; i <= 7; i++)
            {
                ret = ret.Replace(
                  $"%USRBNK{i}%",
                  GetUserBankString(i)
                );
            }

            ret = ret.Replace(
                "%FUNCTIONAL_BANKS%", 
                UserProfile.Instance.MemoryBankStates.Count(x => x.Value).ToString()
            );

            return ret;
        }

        private string GetUserBankString(int number)
        {
            return UserProfile.Instance.IsMemoryBankLocked(number) ? "\uFF04FAIL" : "\uFF40OK";
        }
    }
}
