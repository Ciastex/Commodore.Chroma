using System;
using System.Runtime.InteropServices;

namespace Commodore.Engine.Interop
{
    // Don't call those directly in game code, unless you know
    // exactly what the fuck are you doing. Okay?

    public static class Kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateFile(string fileName, uint desiredAccess, uint shareMode, uint securityAttributes, uint creationDisposition, uint flagsAndAttributes, uint templateFile);
    }
}
