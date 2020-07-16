using Commodore.Framework.Persistence.JsonConfig;
using System.IO;
using System.Reflection;

namespace Commodore.Framework.Managers
{
    public class SettingsManager
    {
        public JsonSettings GraphicsSettings { get; private set; }

        private string RootDirectory { get; }

        public SettingsManager(string configRootDirectory = "Config")
        {
            RootDirectory = configRootDirectory;

            EnsureConfigDirectoryExists();
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            var loc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            GraphicsSettings = new JsonSettings(Path.Combine(loc!, "./Config/graphics.json"));

            GraphicsSettings.GetOrCreate<int?>("ScreenWidth", 1366);
            GraphicsSettings.GetOrCreate("ScreenHeight", 768);
            GraphicsSettings.GetOrCreate("EnableVerticalSync", true);
            GraphicsSettings.GetOrCreate("FullscreenEnabled", false);
            GraphicsSettings.GetOrCreate("FullscreenMode", 0);

            if (GraphicsSettings.Dirty)
                GraphicsSettings.Save();
        }

        private void EnsureConfigDirectoryExists()
        {
            if (!Directory.Exists($"./{RootDirectory}"))
                Directory.CreateDirectory($"./{RootDirectory}");
        }
    }
}
