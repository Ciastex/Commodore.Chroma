using Chroma;
using Chroma.Input.EventArgs;
using Commodore.Engine.Managers;

namespace Commodore.Engine
{
    public class EngineBasedGame : Game
    {
        protected CrashHandler CrashHandler { get; } = new CrashHandler();

        public EngineBasedGame(string contentRootDirectory = "Content") : base()
        {
            DebugLog.Info(".ctor() running...", nameof(EngineBasedGame));
            DebugLog.ForwardToConsole = true;

            // All built-in managers are assigned here, otherwise
            // the initialization is located in specific manger's
            // constructor.
            
            EngineInitialize();
        }

        protected virtual void EngineInitialize()
        {
            G.Window = Window;
            G.GraphicsSettings = Graphics;

            new SettingsManager();
            new DebugManager();

            /*if (G.DebugManager.CanShowConsole && !G.DebugManager.IsConsoleActive)
                G.DebugManager.CreateConsole();
                */
            DebugLog.Info("EngineInitialize() running...", nameof(EngineBasedGame));
        }

        protected virtual void EngineLoad()
        {
            DebugLog.Info("EngineLoad() running...", nameof(EngineBasedGame));
        }

        protected virtual void EngineUnload()
        {
            DebugLog.Info("EngineUnload() running...", nameof(EngineBasedGame));
        }

        protected virtual void EngineKeyPressed(KeyEventArgs e)
        {
        }

        protected virtual void EngineKeyReleased(KeyEventArgs e)
        {
        }

        protected virtual void EngineTextInput(string text)
        {
        }

        protected override void LoadContent()
        {
            G.ContentManager = Content;

            EngineLoad();
            base.LoadContent();
        }

        protected override void KeyPressed(KeyEventArgs e)
        {
            G.DebugManager.KeyPressed(e);
            EngineKeyPressed(e);
        }

        protected override void KeyReleased(KeyEventArgs e)
        {
            G.DebugManager.KeyPressed(e);
            EngineKeyReleased(e);
        }

        protected override void TextInput(TextInputEventArgs e)
        {
            EngineTextInput(e.Text);
        }
    }
}
