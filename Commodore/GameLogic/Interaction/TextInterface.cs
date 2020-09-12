using System.Text;
using Commodore.Framework;
using Commodore.Framework.Extensions;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Persistence;
using Chroma.Input;
using System.Threading.Tasks;

namespace Commodore.GameLogic.Interaction
{
    public class TextInterface
    {
        public static async Task RunProfileConfigWizard()
        {
            Kernel.Instance.Terminal.WriteLine("\nNEW USER DETECTED");

            var processComplete = false;

            var breakKey = UserProfile.Instance.PreferredBreakKey;
            var gfxModeResetKey = UserProfile.Instance.GfxModeResetKey;
            var username = UserProfile.Instance.Username;
            var seed = UserProfile.Instance.Random.Seed;

            while (!processComplete)
            {
                var usernameValid = false;

                while (!usernameValid)
                {
                    var usernameInput = await Kernel.Instance.Terminal.ReadLine($" -> ENTER USERNAME [{username}]: ");

                    if (usernameInput.Length > 16)
                    {
                        Kernel.Instance.Terminal.WriteLine("USERNAME_TOO_LONG".Glitched());
                        continue;
                    }

                    if (!string.IsNullOrEmpty(usernameInput))
                        username = usernameInput;

                    usernameValid = true;
                }

                breakKey = (KeyCode)await Kernel.Instance.Terminal.Read(
                    $" -> PRESS USER PROGRAM KILL KEY [{breakKey}]"
                );
                Kernel.Instance.Terminal.Write("\n");

                gfxModeResetKey = (KeyCode)await Kernel.Instance.Terminal.Read(
                    $" -> PRESS GRAPHICS MODE RECOVERY KEY [{gfxModeResetKey}]"
                );
                Kernel.Instance.Terminal.Write("\n");

                var seedInput = await Kernel.Instance.Terminal.ReadLine($" -> ENTER RNG SEED [{seed}]: ");
                
                if (!string.IsNullOrEmpty(seedInput) && !int.TryParse(seedInput, out seed))
                    seed = seedInput.GetHashCode();

                Kernel.Instance.Terminal.WriteLine("PROFILE SUMMARY");
                Kernel.Instance.Terminal.WriteLine($"  USERNAME: \uFF3F{username}\uFF40");
                Kernel.Instance.Terminal.WriteLine($"   BRK KEY: \uFF3F{breakKey.ToString()}\uFF40");
                Kernel.Instance.Terminal.WriteLine($"  GFXR KEY: \uFF3F{gfxModeResetKey.ToString()}\uFF40");
                Kernel.Instance.Terminal.WriteLine($"  RNG SEED: \uFF3F{seed}\uFF40");

                var input = (KeyCode)0;
                while (input != KeyCode.Y && input != KeyCode.N)
                {
                    Kernel.Instance.Terminal.Write("Is this correct (y/n)? ");
                    input = (KeyCode)await Kernel.Instance.Terminal.Read("");
                }

                if (input == KeyCode.Y)
                {
                    Kernel.Instance.Terminal.WriteLine("Y");
                    Kernel.Instance.Terminal.Write("\n");

                    UserProfile.Instance.CreateBaseFileSystem();
                    UserProfile.Instance.BuildStaticInternetEntities();
                    UserProfile.Instance.SaveToFile();

                    processComplete = true;
                }
                else Kernel.Instance.Terminal.WriteLine("N");
            }

            UserProfile.Instance.Username = username;
            UserProfile.Instance.PreferredBreakKey = breakKey;
            UserProfile.Instance.GfxModeResetKey = gfxModeResetKey;

            UserProfile.Instance.IsInitialized = true;

            Kernel.Instance.Terminal.Write("\nPROFILE_CONFIG_COMPLETE_AWAIT_REBOOT");
            await Kernel.Instance.Terminal.WriteTyped(".....", 650);

            Kernel.Instance.Reboot(true);
        }
    }
}