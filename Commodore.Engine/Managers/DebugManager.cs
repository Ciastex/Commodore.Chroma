using Commodore.Engine.Interop;
using Chroma.Input;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chroma.Input.EventArgs;

namespace Commodore.Engine.Managers
{
    public class DebugManager
    {
        private TextWriter ConsoleStreamWriter { get; set; }
        private Dictionary<KeyCode, Action> DebugActions { get; }

        public bool IsConsoleActive { get; private set; }
        public bool CanShowConsole => Environment.GetCommandLineArgs().Contains("-console");
        public bool ForwardDebugLogToConsole => Environment.GetCommandLineArgs().Contains("-debug");

        public object CommodoreMain { get; private set; }

        public DebugManager()
        {
            if (ForwardDebugLogToConsole)
                DebugLog.ForwardToConsole = true;

            G.DebugManager = this;

            DebugActions = new Dictionary<KeyCode, Action>();
            DebugLog.Info("DebugManager initialized.", nameof(DebugManager));
        }

        internal void KeyPressed(KeyEventArgs e)
        {
            if (DebugActions.ContainsKey(e.KeyCode))
            {
                DebugActions[e.KeyCode]?.Invoke();
            }
        }

        public void BindDebugAction(KeyCode key, Action action)
        {
            if (DebugActions.ContainsKey(key))
            {
                DebugLog.Error($"Tried to add duplicate action for key '{key}'. Fix it.");
                return;
            }

            DebugActions.Add(key, action);
        }

        public void CreateConsole()
        {
            if (!IsConsoleActive)
            {
                Kernel32.AllocConsole();

                var fileHandle = Kernel32.CreateFile(
                    "CONOUT$",
                    0x40000000, // GENERIC_WRITE
                    2,          // FILE_SHARE_WRITE
                    0,
                    3,          // OPEN_EXISTING
                    0,
                    0
                );

                var safeFileHandle = new SafeFileHandle(fileHandle, true);
                var stdOutFileStream = new FileStream(safeFileHandle, FileAccess.Write);
                ConsoleStreamWriter = TextWriter.Synchronized(new StreamWriter(stdOutFileStream) { AutoFlush = true });

                Console.SetOut(ConsoleStreamWriter);
                IsConsoleActive = true;
            }
            else throw new InvalidOperationException("Console already allocated.");
        }

        public void DestroyConsole()
        {
            if (IsConsoleActive)
            {
                ConsoleStreamWriter.Dispose();

                Kernel32.FreeConsole();
                IsConsoleActive = false;
            }
            else throw new InvalidOperationException("Console was not created, cannot destroy.");
        }
    }
}
