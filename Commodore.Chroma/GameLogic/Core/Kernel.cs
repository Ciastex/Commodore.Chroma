#if !DEBUG
    using Commodore.GameLogic.Core.BootSequence;
#endif
using Commodore.Engine;
using Commodore.GameLogic.Core.Hardware;
using Commodore.GameLogic.Core.IO;
using Commodore.GameLogic.Core.IO.Storage;
using Commodore.GameLogic.Display;
using Commodore.GameLogic.Executive.CodeEditor;
using Commodore.GameLogic.Executive.CodeEditor.Events;
using Commodore.GameLogic.Interaction;
using Commodore.GameLogic.Interaction.Shell;
using Commodore.GameLogic.Persistence;
using Chroma.Graphics;
using Chroma.Input;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chroma.Graphics.TextRendering;
using File = Commodore.GameLogic.Core.IO.Storage.File;

namespace Commodore.GameLogic.Core
{
    public class Kernel
    {
        private static readonly TrueTypeFont Font;

        static Kernel()
        {
            Font = G.ContentManager.Load<TrueTypeFont>("Fonts/c64style.ttf", 16);
            //Font.CharacterSpacing = 1f;
        }
        private Kernel() { }

        private static Kernel _instance;
        public static Kernel Instance => _instance ??= new Lazy<Kernel>(() => new Kernel()).Value;

#if !DEBUG
        public BootSequencePlayer BootSequence;
#endif
        public bool IsRebooting { get; private set; }

        public CancellationTokenSource RebootTokenSource { get; private set; }
        public Memory Memory { get; private set; }

        public VGA Vga;
        public Terminal Terminal;

        // REWRITE ARTIFACT
        //public Synthesizer Synthesizer;

        public CodeExecutionLayer CodeExecutionLayer;

        public Editor CodeEditor;
        public TermShell TermShell;

        public FileSystemContext FileSystemContext { get; set; }
        public FileSystemContext LocalFileSystemContext { get; private set; }

        public event EventHandler ColdBootComplete;

        private void Reboot(bool coldBoot)
        {
            IsRebooting = true;

#if !DEBUG
            BootSequence = new BootSequencePlayer(G.ContentManager);
#endif
            if (!coldBoot)
            {
                if (UserProfile.Instance != null && !UserProfile.Instance.Saving)
                    UserProfile.Instance.SaveToFile();
            }
            else UserProfile.Load();

            RebootTokenSource?.Cancel();

            UserProfile.Instance.AutoSave = false;

            RebootTokenSource = new CancellationTokenSource();

            InitializeSystemMemory();
            InitializeVgaAdapter();
            InitializeIoInterfaces();
            InitializeCodeEditor();
            InitializeTermShell();
            // REWRITE ARTIFACT
            // InitializeRxCpu();
            InitializeCodeExecutionLayer();

            LocalFileSystemContext = new FileSystemContext(UserProfile.Instance.RootDirectory);
            FileSystemContext = LocalFileSystemContext;

            if (UserProfile.Instance.IsInitialized)
                UserProfile.Instance.AutoSave = true;

            if (coldBoot)
            {
                ColdBootComplete?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Memory.Poke(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.SoftResetCompleteFlag, 1);
            }

            Vga.UpdateOnce(0f);
        }

        public void DoColdBoot()
        {
            Reboot(true);
        }

        public void DoWarmBoot()
        {
            Reboot(false);
        }

        public void Draw(RenderContext context)
        {
#if DEBUG
            if (UserProfile.Instance.Network.DrawMap)
            {
                UserProfile.Instance.Network.Draw(context);
                return;
            }
#endif
            if (!CodeEditor.IsVisible)
            {
                Vga.Draw(context);
            }
            else
            {
                CodeEditor.Draw(context);
            }
        }

        public void Update(float deltaTime)
        {
            if (CodeEditor.IsVisible)
                CodeEditor.Update(deltaTime);

            Vga.Update(deltaTime);

            Memory.Poke(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.ShiftPressState, Keyboard.IsKeyDown(
                KeyCode.LeftShift) || Keyboard.IsKeyDown(KeyCode.RightShift));
            
            Memory.Poke(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.CtrlPressState, Keyboard.IsKeyDown(
                KeyCode.LeftControl) || Keyboard.IsKeyDown(KeyCode.RightControl));

            Terminal.Update(deltaTime);
            // REWRITE ARTIFACT
            // Synthesizer.Update(deltaTime);
        }

        private void InitializeSystemMemory()
        {
            // Plane 0: System configuration area
            // Plane 1-7: User accessible.

            if (Memory == null)
            {
                Memory = new Memory(SystemConstants.MemorySize, SystemConstants.MemoryPlanes);
            }
            else
            {
                Memory.ClearPlanesAndSetFirst();
            }

            // Unnecessary, but kept here for brevity.
            Memory.SetPlane(0);

            Memory.Poke(SystemMemoryAddresses.BreakKeyScancode, (byte)UserProfile.Instance.PreferredBreakKey);
            Memory.Poke(SystemMemoryAddresses.RevertToTextModeScancode, (byte)UserProfile.Instance.GfxModeResetKey);
            Memory.Poke(SystemMemoryAddresses.CurrentMarginSize, (byte)0);
            Memory.Poke(SystemMemoryAddresses.CurrentPaddingSize, (byte)1);
            Memory.Poke(SystemMemoryAddresses.UpdateOffsetParametersFlag, (byte)1);
            Memory.Poke(SystemMemoryAddresses.CurrentMarginColor, unchecked((int)0xFF00FF00));     // ABGR because i'm retarded
            Memory.Poke(SystemMemoryAddresses.CurrentForegroundColor, unchecked((int)Color.Gray.PackedValue)); // ABGR because i'm retarded
            Memory.Poke(SystemMemoryAddresses.CurrentBackgroundColor, unchecked((int)0xFF000000)); // ABGR because i'm retarded
            Memory.Poke(SystemMemoryAddresses.CursorPositionX, 0);
            Memory.Poke(SystemMemoryAddresses.CursorPositionY, 0);
            Memory.Poke(SystemMemoryAddresses.CtrlPressState, (byte)0);
            Memory.Poke(SystemMemoryAddresses.ShiftPressState, (byte)0);
            Memory.Poke(SystemMemoryAddresses.SoftResetCompleteFlag, 0);
            Memory.Poke(SystemMemoryAddresses.Voice1Frequency, (short)0x0000);
            Memory.Poke(SystemMemoryAddresses.Voice2Frequency, (short)0x0000);
            Memory.Poke(SystemMemoryAddresses.Voice3Frequency, (short)0x0000);
            Memory.Poke(SystemMemoryAddresses.Voice4Frequency, (short)0x0000);
            Memory.Poke(SystemMemoryAddresses.Voice5Frequency, (short)0x0000);
            Memory.Poke(SystemMemoryAddresses.Voice6Frequency, (short)0x0000);
            Memory.Poke(SystemMemoryAddresses.Voice7Frequency, (short)0x0000);
            Memory.Poke(SystemMemoryAddresses.Voice8Frequency, (short)0x0000);
            Memory.Poke(SystemMemoryAddresses.Voice1Generator, (byte)0x02);
            Memory.Poke(SystemMemoryAddresses.Voice2Generator, (byte)0x02);
            Memory.Poke(SystemMemoryAddresses.Voice3Generator, (byte)0x02);
            Memory.Poke(SystemMemoryAddresses.Voice4Generator, (byte)0x02);
            Memory.Poke(SystemMemoryAddresses.Voice5Generator, (byte)0x02);
            Memory.Poke(SystemMemoryAddresses.Voice6Generator, (byte)0x02);
            Memory.Poke(SystemMemoryAddresses.Voice7Generator, (byte)0x02);
            Memory.Poke(SystemMemoryAddresses.Voice8Generator, (byte)0x02);

            Memory.SetPlane(1);
        }

        private void InitializeVgaAdapter()
        {
            if (Vga == null)
            {
                Vga = new VGA(Font);
                Vga.InitialSetUpComplete += VGA_InitialSetUpComplete;
                Vga.FailsafeTriggered += VGA_FailsafeTriggered;
            }
            else
            {
                Vga.ClearScreen(false);
            }
        }

        private void InitializeIoInterfaces()
        {
            if (Terminal == null)
            {
                Terminal = new Terminal(Vga);
            }
            else
            {
                Terminal.ResetInputHistory();
            }

            /* REWRITE ARTIFACT */
            /*
            if (Synthesizer == null)
            {
                Synthesizer = new Synthesizer();
            }*/
        }

        private void InitializeCodeExecutionLayer()
        {
            CodeExecutionLayer = new CodeExecutionLayer();
        }

        /* REWRITE ARTIFACT */
        /*
        private void InitializeRxCpu()
        {
            Processor = new Processor();

            Processor.SetMemoryArea(Memory.Array);
            Processor.SetProgramInfo(0x600, 0);
        }
        */

        private void InitializeCodeEditor()
        {
            if (CodeEditor == null)
            {
                CodeEditor = new Editor(Font);
                CodeEditor.FileSaved += CodeEditor_FileSaved;
            }
            else
            {
                CodeEditor.Reset();
            }
        }

        private void InitializeTermShell()
        {
            if (TermShell == null)
                TermShell = new TermShell();
        }

        private void VGA_FailsafeTriggered(object sender, EventArgs e)
        {
            Terminal.WriteLine("abnormal vga parameters triggered a failsafe reset");
        }

        private async void VGA_InitialSetUpComplete(object sender, EventArgs e)
        {
#if !DEBUG
            BootSequence.Build("RegularBoot");
            await BootSequence.TryPerformSequence();
#endif
            if (!UserProfile.Instance.IsInitialized)
            {
                await TextInterface.RunProfileConfigWizard();
                return;
            }

            TextInterface.PrintWelcomeBanner(
                Memory.PeekBool(
                    SystemConstants.SystemMemoryPlane,
                    SystemMemoryAddresses.SoftResetCompleteFlag
                )
            );

            IsRebooting = false;

            while (!IsRebooting)
            {
                var customPromptSuccess = false;
                if (File.Exists("/etc/prompt"))
                {
                    try
                    {
                        await CodeExecutionLayer.ExecuteProgram(
                            Encoding.UTF8.GetString(File.Get("/etc/prompt").Data)
                        );

                        customPromptSuccess = true;
                    }
                    catch { }
                }

                if (!customPromptSuccess)
                {
                    var host = "localhost";

                    if (UserProfile.Instance.Network.Cursor.CurrentConnectedDevice != null)
                    {
                        host = UserProfile.Instance.Network.Cursor.CurrentConnectedDevice.IP.Address;
                    }

                    Terminal.Write($"{FileSystemContext.WorkingDirectory.GetAbsolutePath()} : {host} $ ");
                }

                var str = await Terminal.ReadLine(string.Empty, RebootTokenSource.Token);

                if (string.IsNullOrWhiteSpace(str))
                    continue;

                await Task.Run(() => TermShell.HandleCommand(str));
            }
        }

        public void KeyPressed(KeyCode keyCode, KeyModifiers modifiers)
        {
            if (keyCode == KeyCode.F8)
            {
                UserProfile.Instance.Network.DrawMap = !UserProfile.Instance.Network.DrawMap;
            }

#if DEBUG
            if (UserProfile.Instance.Network.DrawMap)
            {
                if (keyCode == KeyCode.Left)
                {
                    UserProfile.Instance.Network.MapOffsetH -= 2;
                }
                else if (keyCode == KeyCode.Right)
                {
                    UserProfile.Instance.Network.MapOffsetH += 2;
                }

                if (keyCode == KeyCode.Up)
                {
                    UserProfile.Instance.Network.MapOffsetV -= 2;
                }
                else if (keyCode == KeyCode.Down)
                {
                    UserProfile.Instance.Network.MapOffsetV += 2;
                }

                if (keyCode == KeyCode.Plus)
                {
                    UserProfile.Instance.Network.MapScale++;
                }
                else if (keyCode == KeyCode.Minus)
                {
                    UserProfile.Instance.Network.MapScale--;
                }

                return;
            }
#endif
            if (!CodeEditor.IsVisible)
            {
                if ((byte)keyCode == Memory.Peek8(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.BreakKeyScancode))
                {
                    if (CodeExecutionLayer.IsExecuting)
                        CodeExecutionLayer.Interpreter.BreakExecution = true;

                    if (Memory.PeekBool(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.ShiftPressState) && !IsRebooting)
                        Reboot(false);
                }

                Terminal.KeyPressed(keyCode, modifiers);
            }
            else
            {
                CodeEditor.KeyPressed(keyCode, modifiers);
            }
        }

        public void TextInput(char character)
        {
            if (!CodeEditor.IsVisible)
            {
                Terminal.TextInput(character);
            }
            else
            {
                CodeEditor.TextInput(character);
            }
        }

        private void CodeEditor_FileSaved(object sender, FileSavedEventArgs e)
        {
            var file = File.Create(e.FilePath, true);
            file.SetData(e.Contents);

            if (e.FilePath.StartsWith("/bin/"))
                file.Attributes = FileAttributes.Executable;
        }
    }
}