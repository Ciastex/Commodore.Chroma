using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;
using Chroma.Graphics;
using Chroma.Graphics.Accelerated;
using Chroma.Input;
using Chroma.Input.EventArgs;
using Chroma.Windowing.EventArgs;
using Commodore.Engine;
using Commodore.Engine.Graphics;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Generation;
using Commodore.GameLogic.Persistence;
using Color = Chroma.Graphics.Color;

namespace Commodore
{
    public class CommodoreMain : EngineBasedGame
    {
        private PixelShader _shader;
        private RenderTarget _frameBuffer;
        private bool _shaderEnabled = true;

        protected override void EngineLoad()
        {
            GraphicsManager.LimitFramerate = false;
            base.EngineLoad();

            try
            {
                DebugLog.Info("Loading usernames...");
                using (var sr = new StreamReader(Content.Open("GenerationStuff/usernames.txt")))
                {
                    UsernameGenerator.AddRange(
                        sr.ReadToEnd().Split('\n')
                    );
                }
            }
            catch (Exception e)
            {
                DebugLog.Exception(e, "Failed to load usernames for generator. Will fall back to random usernames.");
            }
            
            Window.QuitRequested += WindowOnQuitRequested;
        }

        private void WindowOnQuitRequested(object sender, CancelEventArgs e)
        {
            while (UserProfile.Instance.Saving)
                Thread.Sleep(1);
        
            UserProfile.Instance.SaveToFile();
        }

        protected override void EngineInitialize()
        {
            base.EngineInitialize();

            ApplyGraphicsSettings();
            Window.CenterScreen();

            CreateFrameBuffer();
            InitializePixelShader();

            G.DebugManager.BindDebugAction(KeyCode.F9, () =>
            {
                _shaderEnabled = !_shaderEnabled;
            });
            
            Kernel.Instance.DoColdBoot();
        }

        protected override void Update(float deltaTime)
        {
            Window.Title = Window.FPS.ToString();
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

        protected override void EngineKeyPressed(KeyEventArgs e)
        {
            Kernel.Instance.KeyPressed(e.KeyCode, e.Modifiers);
        }

        protected override void EngineTextInput(string text)
        {
            Kernel.Instance.TextInput(text[0]);
        }

        private void ApplyGraphicsSettings()
        {
            DebugLog.Info("Applying graphics settings...", nameof(CommodoreMain));

            Graphics.VerticalSyncMode = G.SettingsManager.GraphicsSettings.GetItem<bool>("EnableVerticalSync") ? VerticalSyncMode.Retrace : VerticalSyncMode.None;
            
            Window.Size = new Size(
                G.SettingsManager.GraphicsSettings.GetItem<int>("ScreenWidth"),
                G.SettingsManager.GraphicsSettings.GetItem<int>("ScreenHeight")
            );

            if (G.SettingsManager.GraphicsSettings.GetItem<bool>("FullscreenEnabled"))
            {
                switch ((FullscreenMode)G.SettingsManager.GraphicsSettings.GetItem<int>("FullscreenMode"))
                {
                    case FullscreenMode.Exclusive:
                    case FullscreenMode.BorderlessWindow:
                        Window.GoFullscreen(true);
                        break;

                    default: throw new Exception("Invalid fullscreen display mode.");
                }
            }
        }

        private void InitializePixelShader()
        {
            DebugLog.Info("Initializing main pixel shader...", nameof(CommodoreMain));

            _shader = Content.Load<PixelShader>("Shaders/CrtScreen.glsl");
        }

        private void CreateFrameBuffer()
        {
            DebugLog.Info("Creating framebuffer for main pixel shader...", nameof(CommodoreMain));
            _frameBuffer = new RenderTarget(Window.Size);
        }
    }
}