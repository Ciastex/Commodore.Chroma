using Commodore.Engine.Graphics;
using Commodore.Engine.Persistence.JsonConfig;
using System.IO;

namespace Commodore.Engine.Managers
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

            G.SettingsManager = this;

            DebugLog.Info("SettingsManager initialized.");
        }

        protected virtual void InitializeSettings()
        {
            GraphicsSettings = new JsonSettings("./Config/graphics.json");

            GraphicsSettings.GetOrCreate("ScreenWidth", 1366);
            GraphicsSettings.GetOrCreate("ScreenHeight", 768);
            GraphicsSettings.GetOrCreate("EnableVerticalSync", true);
            GraphicsSettings.GetOrCreate("FullscreenEnabled", true);
            GraphicsSettings.GetOrCreate("FullscreenMode", (int)FullscreenMode.BorderlessWindow);

            if (GraphicsSettings.Dirty)
                GraphicsSettings.Save();

            DebugLog.Info("Loaded graphics settings.");
        }

        private void EnsureConfigDirectoryExists()
        {
            if (!Directory.Exists($"./{RootDirectory}"))
                Directory.CreateDirectory($"./{RootDirectory}");
        }
    }
}
