using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;
using Chroma;
using Chroma.Graphics;
using Chroma.Graphics.Accelerated;
using Chroma.Input;
using Chroma.Input.EventArgs;
using Chroma.Windowing.EventArgs;
using Commodore.Framework;
using Commodore.Framework.Managers;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Generation;
using Commodore.GameLogic.Persistence;
using Color = Chroma.Graphics.Color;

namespace Commodore
{
    public class CommodoreMain : Game
    {
        private PixelShader _shader;
        private RenderTarget _frameBuffer;
        private bool _kernelStarted = false;
        private bool _shaderEnabled = true;

        public CommodoreMain() : base(false)
        {
            G.ContentProvider = Content;
            G.Window = Window;
            G.GraphicsSettings = Graphics;

            G.SettingsManager = new SettingsManager();
            
            G.DebugManager = new DebugManager();
            G.DebugManager.BindDebugAction(KeyCode.F9, () => { _shaderEnabled = !_shaderEnabled; });
            
            Window.QuitRequested += WindowOnQuitRequested;            
        }

        protected override void LoadContent()
        {
            ApplyGraphicsSettings();

            using (var sr = new StreamReader(Content.Open("GenerationStuff/usernames.txt")))
            {
                UsernameGenerator.AddRange(
                    sr.ReadToEnd().Split('\n')
                );
            }
            
            _frameBuffer = new RenderTarget(Window.Size);
            _shader = Content.Load<PixelShader>("Shaders/CrtScreen.glsl");
        }

        private void WindowOnQuitRequested(object sender, CancelEventArgs e)
        {
            while (UserProfile.Instance.Saving)
                Thread.Sleep(1);

            UserProfile.Instance.SaveToFile();
        }

        protected override void Update(float deltaTime)
        {
            if (!_kernelStarted)
            {
                _kernelStarted = true;
                Kernel.Instance.DoColdBoot();
            }
            
            Window.Title = $"Project Commodore [{Window.FPS} FPS]";
            Kernel.Instance.Update(deltaTime);
        }

        protected override void Draw(RenderContext context)
        {
            context.RenderTo(_frameBuffer, () =>
            {
                context.Clear(Color.Black);
                Kernel.Instance.Draw(context);
            });

            if (_shaderEnabled)
            {
                _shader.Activate();
                _shader.SetUniform("screenSize",
                    new Vector2(
                        Window.Size.Width,
                        Window.Size.Height
                    )
                );
                _shader.SetUniform("scanlineDensity", 2f);
                _shader.SetUniform("blurDistance", .375f);
            }

            context.DrawTexture(_frameBuffer, Vector2.Zero, Vector2.One, Vector2.Zero, 0);
            Shader.Deactivate();
        }

        protected override void KeyPressed(KeyEventArgs e)
        {
            G.DebugManager.KeyPressed(e);
            Kernel.Instance.KeyPressed(e.KeyCode, e.Modifiers);
        }

        protected override void TextInput(TextInputEventArgs e)
        {
            Kernel.Instance.TextInput(e.Text[0]);
        }

        private void ApplyGraphicsSettings()
        {
            Graphics.VerticalSyncMode = G.SettingsManager.GraphicsSettings.GetItem<bool>("EnableVerticalSync")
                ? VerticalSyncMode.Retrace
                : VerticalSyncMode.None;

            Window.Size = new Size(
                G.SettingsManager.GraphicsSettings.GetItem<int>("ScreenWidth"),
                G.SettingsManager.GraphicsSettings.GetItem<int>("ScreenHeight")
            );
            
            Window.CenterScreen();

            if (G.SettingsManager.GraphicsSettings.GetItem<bool>("FullscreenEnabled"))
            {
                switch (G.SettingsManager.GraphicsSettings.GetItem<int>("FullscreenMode"))
                {
                    case 0:
                        Window.GoFullscreen(false);
                        break;
                    
                    case 1:
                        Window.GoFullscreen(true);
                        break;

                    default: throw new Exception("Invalid fullscreen display mode.");
                }
            }
        }
    }
}