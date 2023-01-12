using System;
using System.Collections.Generic;
using System.IO;

namespace Commodore.Engine
{
    public class CrashHandler
    {
        private List<string> Quips { get; } = new List<string>
        {
            "Developer sucks, 0/10 game, refund recommended",
            "I don't feel too well!",
            "WHY DID YOU DO THIS?!",
            "Well, he just took this gun, aaand... *bang*",
            "OH GOD OH FUCK OH GOD OH FUCK",
            "In Soviet Russia the exception throws you!",
            "BuT My CoDe Is PeRfEcT HoW CaN It FaIl?!!?!!",
            "Rest in pizzas, I guess",
            "Why are you runni... Not anymore I suppose",
            "They told me I can't _run_ from problems, guess they were right",
            "Stop throwing combustible lemons at me!",
            "Now that's what I call a 4th wall break"
        };

        private Random Random { get; }
        private string RandomQuip => Quips[Random.Next(0, Quips.Count)];

        public Action<UnhandledExceptionEventArgs> CustomExceptionActions;

        internal CrashHandler()
        {
            Random = new Random();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public void SetCustomExceptionActions(Action<UnhandledExceptionEventArgs> action)
        {
            CustomExceptionActions = action;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            using (var sw = new StreamWriter("fuckup.log"))
            {
                sw.WriteLine(RandomQuip);
                sw.WriteLine("------------------");
                sw.WriteLine(ex);

                sw.WriteLine($"Is CLR terminating? - {e.IsTerminating}");

                while (ex.InnerException != null)
                {
                    sw.WriteLine("--- <--- INNER EXCEPTION FOR THE EXCEPTION IMMEDIATELY ABOVE ---> ---");
                    sw.WriteLine(ex);

                    ex = ex.InnerException;
                }
            }

            CustomExceptionActions?.Invoke(e);
        }
    }
}
