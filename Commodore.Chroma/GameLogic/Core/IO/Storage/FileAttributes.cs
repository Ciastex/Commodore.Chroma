using System;

namespace Commodore.GameLogic.Core.IO.Storage
{
    [Flags]
    public enum FileAttributes
    {
        Executable = 0x1,
        Hidden = 0x2
    }
}
