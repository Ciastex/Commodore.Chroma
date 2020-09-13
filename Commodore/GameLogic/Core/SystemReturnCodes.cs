namespace Commodore.GameLogic.Core
{
    public static class SystemReturnCodes
    {
        public const int Success = 0;

        public static class Network
        {
            public const int ServiceNodeInactive = 1;
            public const int ServiceNodeActive = 1;
            public const int NotADevice = -1;
            public const int ShellDisabled = -2;
            public const int ContextIsLocal = -3;
            public const int NotLinked = -4;
            public const int AlreadyLinked = -5;
            public const int NoRouteToHost = -6;
            public const int ShellStillOpen = -7;
            public const int BoundToNode = -8;
            public const int ServiceNodeUnresponsive = -9;
            public const int NotANode = -10;
            public const int ShellNotOpen = -11;
        }

        public static class FileSystem
        {
            // Todo
        }
    }
}