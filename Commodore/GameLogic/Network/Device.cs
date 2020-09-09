using System.Collections.Generic;
using System.Linq;
using Commodore.GameLogic.Core.IO.Storage;
using Commodore.GameLogic.Network.Services;

namespace Commodore.GameLogic.Network
{
    public abstract class Device
    {
        public abstract string Address { get; protected set; }
        
        public Directory RootDirectory { get; }  = new Directory("/", null);
        public Dictionary<ushort, Service> Services { get; } = new Dictionary<ushort, Service>();
    }
}