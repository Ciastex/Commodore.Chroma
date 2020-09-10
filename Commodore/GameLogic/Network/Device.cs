using System.Collections.Generic;
using System.Linq;
using Commodore.GameLogic.Core.IO.Storage;
using Commodore.GameLogic.Network.Services;

namespace Commodore.GameLogic.Network
{
    public abstract class Device
    {
        public Address Address { get; set; }

        public bool ShellEnabled { get; protected set; } = true;
        public Directory RootDirectory { get; }  = new Directory("/", null);
        
        public Dictionary<byte, Service> Services { get; } = new Dictionary<byte, Service>();
    }
}