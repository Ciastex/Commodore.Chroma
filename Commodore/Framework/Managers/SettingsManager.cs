using System.IO;

namespace Commodore.Framework.Managers
{
    public class SettingsManager
    {
        private string RootDirectory { get; }

        public int ScreenWidth { get; private set; } = 1366;
        public int ScreenHeight { get; private set; } = 768;
        public bool EnableVerticalSync { get; private set; } = true;
        public bool FullscreenEnabled { get; private set; } = true;
        public bool IsBorderless { get; private set; } = true;

        public SettingsManager(string configRootDirectory = "Config")
        {
            RootDirectory = configRootDirectory;

            EnsureConfigDirectoryExists();
        }

        private void EnsureConfigDirectoryExists()
        {
            if (!Directory.Exists($"./{RootDirectory}"))
                Directory.CreateDirectory($"./{RootDirectory}");
        }
    }
}
