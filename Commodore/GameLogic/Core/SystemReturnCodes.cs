namespace Commodore.GameLogic.Core
{
    public static class SystemReturnCodes
    {
        public const int Success = 0;

        public static class FileSystem
        {
            public const int FileDoesNotExist = 0x10000001;
            public const int FileAlreadyExists = 0x10000002;
            public const int AccessDenied = 0x10000003;
        }
        
        public static class Network
        {
            public const int ServiceNodeInactive = 1;
            public const int ServiceNodeActive = 2;
            public const int NotADevice = 0x20000001;
            public const int ShellDisabled = 0x20000002;
            public const int ContextIsLocal = 0x20000003;
            public const int NotLinked = 0x20000004;
            public const int AlreadyLinked = 0x20000005;
            public const int NoRouteToHost = 0x20000006;
            public const int NotANode = 0x20000007;
            public const int BoundToNode = 0x20000008;
            public const int ServiceNodeUnresponsive = 0x20000009;
            public const int ShellStillOpen = 0x2000000A;
            public const int ShellNotOpen = 0x2000000B;
        }

        public static class Kernel
        {
            public const int ProcessSpaceExhausted = 0x30000001;
        }
    }
}