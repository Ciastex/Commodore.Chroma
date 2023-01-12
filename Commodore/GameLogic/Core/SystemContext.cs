using System;
using Commodore.GameLogic.Core.IO.Storage;
using Commodore.GameLogic.Network;

namespace Commodore.GameLogic.Core
{
    [Serializable]
    public class SystemContext
    {
        public Device RemoteDevice { get; }
        public Directory RootDirectory { get; }

        public Directory WorkingDirectory { get; set; }

        public bool IsLocal => RemoteDevice == null;

        public SystemContext(Directory rootDirectory)
        {
            RootDirectory = rootDirectory;
            WorkingDirectory = RootDirectory;
        }

        public SystemContext(Device remoteDevice)
            : this(remoteDevice.RootDirectory)
        {
            RemoteDevice = remoteDevice;
        }
    }
}