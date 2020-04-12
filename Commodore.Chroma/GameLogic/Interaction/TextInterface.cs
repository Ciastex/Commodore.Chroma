using System.Text;
using Commodore.Engine;
using Commodore.Engine.FrameworkExtensions;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Persistence;
using Commodore.GameLogic.World;
using Chroma.Input;
using System.Threading.Tasks;

namespace Commodore.GameLogic.Interaction
{
    public class TextInterface
    {
        public static void PrintWelcomeBanner(bool postSoftReset)
        {
            var welcomeMsg1 = "*** ENSIGN SUPER-32 CPLX SYSTEM V0.1 ***";
            var welcomeMsg2 = $"{(Kernel.Instance.Memory.Planes.Count * SystemConstants.MemorySize) / 1024}KB MEMORY";

            Kernel.Instance.Vga.CursorX = (Kernel.Instance.Vga.TotalColumns / 2) - (welcomeMsg1.Length / 2);
            Kernel.Instance.Vga.CursorY++;
            Kernel.Instance.Terminal.WriteLine(welcomeMsg1);

            Kernel.Instance.Vga.CursorX = (Kernel.Instance.Vga.TotalColumns / 2) - (welcomeMsg2.Length / 2);
            Kernel.Instance.Terminal.WriteLine(welcomeMsg2);

            if (postSoftReset)
                Kernel.Instance.Terminal.WriteLine("\nSOFT RESET COMPLETE");

            Kernel.Instance.Terminal.WriteLine("\nREADY.");
        }

        // todo refactor this
        public static async Task RunProfileConfigWizard()
        {
            Kernel.Instance.Terminal.WriteLine("\nNEW USER DETECTED");

            var processComplete = false;

            var breakKey = KeyCode.Pause;
            var gfxModeResetKey = KeyCode.F12;
            var username = "lazarus";
            var netSeed = (int)Time.Stamp; 

            while (!processComplete)
            {
                var usernameValid = false;

                while (!usernameValid)
                {
                    username = await Kernel.Instance.Terminal.ReadLine(" -> USERNAME: ", Kernel.Instance.RebootTokenSource.Token);

                    if (username.Length > 16)
                    {
                        Kernel.Instance.Terminal.WriteLine("USERNAME_TOO_LONG".Glitched());
                        continue;
                    }
                    else if (username.Length == 0)
                    {
                        Kernel.Instance.Terminal.WriteLine("USERNAME_EMPTY".Glitched());
                    }

                    usernameValid = true;
                }

                breakKey = (KeyCode)await Kernel.Instance.Terminal.Read(" -> PRESS_SCRIPT_BREAK_KEY", Kernel.Instance.RebootTokenSource.Token);
                Kernel.Instance.Terminal.Write("\n");

                gfxModeResetKey = (KeyCode)await Kernel.Instance.Terminal.Read(" -> PRESS_GFXMODE_RESET_KEY", Kernel.Instance.RebootTokenSource.Token);
                Kernel.Instance.Terminal.Write("\n");

                var networkSeedInput = await Kernel.Instance.Terminal.ReadLine(" -> NETWORK_SEED: ", Kernel.Instance.RebootTokenSource.Token);

                if (!string.IsNullOrEmpty(networkSeedInput))
                {
                    if (!int.TryParse(networkSeedInput, out netSeed))
                    {
                        netSeed = networkSeedInput.GetHashCode();
                    }
                }

                Kernel.Instance.Terminal.WriteLine("User input summary:");
                Kernel.Instance.Terminal.WriteLine($"  Username: {username}");
                Kernel.Instance.Terminal.WriteLine($"  Break key: {breakKey.ToString()}");
                Kernel.Instance.Terminal.WriteLine($"  Graphics reset key: {gfxModeResetKey.ToString()}");
                Kernel.Instance.Terminal.WriteLine($"  Network seed: {netSeed}");

                var input = (KeyCode)0;
                while (input != KeyCode.Y && input != KeyCode.N)
                {
                    Kernel.Instance.Terminal.Write("Is this correct (y/n)? ");
                    input = (KeyCode)await Kernel.Instance.Terminal.Read("", Kernel.Instance.RebootTokenSource.Token);
                }

                if (input == KeyCode.Y)
                {
                    Kernel.Instance.Terminal.WriteLine("Y");

                    UserProfile.Instance.RootDirectory.AddNewDirectory("bin");
                    UserProfile.Instance.RootDirectory.AddNewDirectory("home");
                    UserProfile.Instance.RootDirectory.AddNewDirectory("lib");
                    var binDirectory = (UserProfile.Instance.RootDirectory.Children["bin"] as Core.IO.Storage.Directory);

                    binDirectory.AddNewFile("ls").SetData(
                        Encoding.UTF8.GetString(G.ContentManager.Read("Sources/BasePrograms/ls"))
                    ).Attributes = Core.IO.Storage.FileAttributes.Executable;

                    binDirectory.AddNewFile("cat").SetData(
                        Encoding.UTF8.GetString(G.ContentManager.Read("Sources/BasePrograms/cat"))
                    ).Attributes = Core.IO.Storage.FileAttributes.Executable;

                    binDirectory.AddNewFile("cp").SetData(
                        Encoding.UTF8.GetString(G.ContentManager.Read("Sources/BasePrograms/cp"))
                    ).Attributes = Core.IO.Storage.FileAttributes.Executable;

                    binDirectory.AddNewFile("rm").SetData(
                        Encoding.UTF8.GetString(G.ContentManager.Read("Sources/BasePrograms/rm"))
                    ).Attributes = Core.IO.Storage.FileAttributes.Executable;

                    binDirectory.AddNewFile("cd").SetData(
                        Encoding.UTF8.GetString(G.ContentManager.Read("Sources/BasePrograms/cd"))
                    ).Attributes = Core.IO.Storage.FileAttributes.Executable;

                    binDirectory.AddNewFile("help").SetData(
                        Encoding.UTF8.GetString(G.ContentManager.Read("Sources/BasePrograms/help"))
                    ).Attributes = Core.IO.Storage.FileAttributes.Executable;

                    binDirectory.AddNewFile("crawl").SetData(
                        Encoding.UTF8.GetString(G.ContentManager.Read("Sources/BasePrograms/crawl"))
                    ).Attributes = Core.IO.Storage.FileAttributes.Executable;

                    UserProfile.Instance.SaveToFile();
                    processComplete = true;
                }
                else Kernel.Instance.Terminal.WriteLine("N");
            }

            UserProfile.Instance.UserName = username;
            UserProfile.Instance.PreferredBreakKey = breakKey;
            UserProfile.Instance.GfxModeResetKey = gfxModeResetKey;

            UserProfile.Instance.Network = new Network(netSeed);
            UserProfile.Instance.IsInitialized = true;

            Kernel.Instance.Terminal.WriteLine("\nPROFILE_CONFIG_COMPLETE_AWAIT_REBOOT");
            await Kernel.Instance.Terminal.WriteTyped(".....", 650);

            Kernel.Instance.DoWarmBoot();
        }
    }
}
