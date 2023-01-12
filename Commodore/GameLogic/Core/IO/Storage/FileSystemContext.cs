using System;

namespace Commodore.GameLogic.Core.IO.Storage
{
    [Serializable]
    public class FileSystemContext
    {
        public Directory RootDirectory { get; }
        public Directory WorkingDirectory { get; set; }

        public FileSystemContext(Directory rootDirectory)
        {
            RootDirectory = rootDirectory;
            WorkingDirectory = rootDirectory;
        }
    }
}
