#if !DEBUG
using Commodore.GameLogic.Core.BootSequence;
#endif
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering;
using Chroma.Input;
using Commodore.Framework;
using Commodore.Framework.Extensions;
using Commodore.GameLogic.Core.Exceptions;
using Commodore.GameLogic.Core.Hardware;
using Commodore.GameLogic.Core.IO;
using Commodore.GameLogic.Core.IO.Storage;
using Commodore.GameLogic.Display;
using Commodore.GameLogic.Executive.CodeEditor;
using Commodore.GameLogic.Executive.CodeEditor.Events;
using Commodore.GameLogic.Interaction;
using Commodore.GameLogic.Interaction.Shell;
using Commodore.GameLogic.Persistence;

namespace Commodore.GameLogic.Core
{
    public class Kernel
    {
        private static readonly TrueTypeFont Font;

        static Kernel()
        {
            Font = G.ContentProvider.Load<TrueTypeFont>(
                "Fonts/c64style.ttf",
                16,
                "`1234567890-=qwertyuiop[]asdfghjkl;'\\zxcvbnm,./~!@#$%^&*()_+QWERTYUIOP{}ASDFGHJKL:\"|ZXCVBNM<>?" +
                "\ue05e\ue06a\ue076\ue05f\ue06b\ue077\ue060\ue06c\ue078\ue061\ue06d\ue079\ue062\ue06e\ue07a\ue063" +
                "\ue06f\ue07b\ue064\ue070\ue07c\ue065\ue071\ue07d\ue066\ue072\ue07e\ue067\ue073\ue07f\ue068\ue074" +
                "\ue069\ue075\ue2a0\ue0ac\ue0b8\ue0a1\ue0ad\ue0b9\ue0a2\ue0ae\ue0ba\ue0a3\ue0af\ue0bb\ue0a4\ue0b0" +
                "\ue0bc\ue0a5\ue0b1\ue0bd\ue0a6\ue0b2\ue0be\ue0a7\ue0b3\ue0bf\ue0a8\ue0b4\ue05c\ue0a9\ue0b5\ue0aa" +
                "\ue0b6\ue0ab\ue0b7 "
            );
        }

        private Kernel()
        {
        }

        private static Kernel _instance;
        public static Kernel Instance => _instance ??= new Lazy<Kernel>(() => new Kernel()).Value;

        private Notification _currentNotification;

#if !DEBUG
        public BootSequencePlayer BootSequence;
#endif
        public bool IsRebooting { get; private set; }

        public Memory Memory { get; private set; }

        public VGA Vga;
        public Terminal Terminal;

        public ProcessManager ProcessManager;

        public Editor CodeEditor;
        public TermShell TermShell;

        public FileSystemContext FileSystemContext { get; set; }
        public FileSystemContext LocalFileSystemContext { get; private set; }
        
        public Queue<Notification> Notifications { get; private set; } = new Queue<Notification>();

        public void Reboot(bool isWarmBoot)
        {
            if (_currentNotification != null)
            {
                _currentNotification.IsActive = false;
                _currentNotification = null;
            }
            Notifications.Clear();

            IsRebooting = true;

            if (isWarmBoot)
            {
                if (UserProfile.Instance != null && !UserProfile.Instance.Saving)
                    UserProfile.Instance.SaveToFile();
            }
            else
            {
                UserProfile.Load();
                UserProfile.Instance.AutoSave = false;
            }
#if !DEBUG
            BootSequence = new BootSequencePlayer();
#endif

            InitializeSystemMemory();
            InitializeVgaAdapter();
            InitializeIoInterfaces();
            InitializeCodeEditor();
            InitializeTermShell();
            InitializeCodeExecutionLayer();

            LocalFileSystemContext = new FileSystemContext(UserProfile.Instance.RootDirectory);
            FileSystemContext = LocalFileSystemContext;

            if (!isWarmBoot)
            {
                if (UserProfile.Instance.IsInitialized)
                    UserProfile.Instance.AutoSave = true;
            }

            if (isWarmBoot)
            {
                Memory.Poke(SystemMemoryAddresses.SoftResetCompleteFlag, (byte)1);
            }
        }

        public void Draw(RenderContext context)
        {
            if (!CodeEditor.IsVisible)
            {
                Vga.Draw(context);
            }
            else
            {
                CodeEditor.Draw(context);
            }

            if (_currentNotification != null)
                _currentNotification.Draw(context);
        }

        public void Update(float delta)
        {
            if (Notifications.Count > 0 && _currentNotification == null)
            {
                _currentNotification = Notifications.Dequeue();
                _currentNotification.IsActive = true;
            }

            if (_currentNotification != null)
            {
                _currentNotification.Update(delta);
                
                if (!_currentNotification.IsActive)
                    _currentNotification = null;
            }
            
            if (CodeEditor.IsVisible)
                CodeEditor.Update(delta);

            Vga.Update(delta);

            Memory.Poke(
                SystemMemoryAddresses.ShiftPressState,
                Keyboard.IsKeyDown(KeyCode.LeftShift) || Keyboard.IsKeyDown(KeyCode.RightShift)
            );

            Memory.Poke(
                SystemMemoryAddresses.CtrlPressState,
                Keyboard.IsKeyDown(KeyCode.LeftControl) || Keyboard.IsKeyDown(KeyCode.RightControl)
            );

            Memory.Poke(
                SystemMemoryAddresses.AltPressState,
                Keyboard.IsKeyDown(KeyCode.LeftAlt) || Keyboard.IsKeyDown(KeyCode.RightAlt)
            );

            Terminal.Update(delta);
        }

        public void Notify(string text)
        {
            Notifications.Enqueue(new Notification(text));
        }

        private void InitializeSystemMemory()
        {
            if (Memory == null)
            {
                Memory = new Memory(SystemConstants.MemorySize);
            }
            else
            {
                Memory.Clear();
            }

            Memory.Poke(SystemMemoryAddresses.BreakKeyScancode, (byte)UserProfile.Instance.PreferredBreakKey);
            Memory.Poke(SystemMemoryAddresses.RevertToTextModeScancode, (byte)UserProfile.Instance.GfxModeResetKey);
            Memory.Poke(SystemMemoryAddresses.CurrentMarginSize, (byte)0);
            Memory.Poke(SystemMemoryAddresses.CurrentPaddingSize, (byte)1);
            Memory.Poke(SystemMemoryAddresses.UpdateOffsetParametersFlag, (byte)1);
            Memory.Poke(
                SystemMemoryAddresses.CurrentMarginColor,
                unchecked((int)0xFF00FF00)
            ); // ABGR

            Memory.Poke(
                SystemMemoryAddresses.CurrentForegroundColor,
                unchecked((int)Color.Gray.PackedValue)
            );

            Memory.Poke(
                SystemMemoryAddresses.CurrentBackgroundColor,
                unchecked((int)0xFF000000)
            );

            Memory.Poke(SystemMemoryAddresses.CursorPositionX, 0);
            Memory.Poke(SystemMemoryAddresses.CursorPositionY, 0);
            Memory.Poke(SystemMemoryAddresses.CtrlPressState, (byte)0);
            Memory.Poke(SystemMemoryAddresses.ShiftPressState, (byte)0);
            Memory.Poke(SystemMemoryAddresses.AltPressState, (byte)0);
            Memory.Poke(SystemMemoryAddresses.SoftResetCompleteFlag, (byte)0);
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
                Vga.ResetInitialization();
                Vga.ClearScreen(false);
            }
        }

        private void InitializeIoInterfaces()
        {
            Terminal?.CancelInput();
            Terminal?.InputHistory?.Clear();
            Terminal = null;

            Terminal = new Terminal(Vga);
        }

        private void InitializeCodeExecutionLayer()
        {
            if (ProcessManager == null)
            {
                ProcessManager = new ProcessManager();
            }
            else
            {
                ProcessManager.KillAll();
            }
        }

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
            IsRebooting = false;
            
            if (!UserProfile.Instance.IsInitialized)
            {
                await TextInterface.RunProfileConfigWizard();
                return;
            }

            await TryRunSystemProgram("/etc/startup");
            
            try
            {
                while (true)
                {
                    var customPromptSuccess = await TryRunSystemProgram("/etc/prompt");

                    if (!customPromptSuccess)
                    {
                        Terminal.Write($"{FileSystemContext.WorkingDirectory.GetAbsolutePath()} $ ");
                    }

                    var str = await Terminal.ReadLine(string.Empty);

                    if (string.IsNullOrWhiteSpace(str))
                        continue;

                    await TermShell.HandleCommand(str);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async Task<bool> TryRunSystemProgram(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                var pid = await ProcessManager.ExecuteProgram(
                    Encoding.UTF8.GetString(File.Get(path).Data),
                    path
                );

                await ProcessManager.WaitForProgram(pid);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public void KeyPressed(KeyCode keyCode, KeyModifiers modifiers)
        {
            if (keyCode == KeyCode.F4)
            {
                Notify("lorem ipsum dolor sit amet\nonsectetur adipisici elit\n\nensign dont pay me enough");
                Notify("to remove this shit");
                Notify("from tech preview");
                Notify("so enjoy the toasts, clients");
                Notify("om-nom-nom");
            }
            if (!CodeEditor.IsVisible)
            {
                if ((byte)keyCode == Memory.Peek8(SystemMemoryAddresses.BreakKeyScancode))
                {
                    ProcessManager.KillAll();

                    if (Memory.PeekBool(SystemMemoryAddresses.ShiftPressState) && !IsRebooting)
                    {
                        Reboot(true);
                        return;
                    }
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
            if (IsRebooting)
                return;

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