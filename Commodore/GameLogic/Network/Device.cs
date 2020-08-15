using System.Collections.Generic;
using System.Linq;
using Commodore.GameLogic.Core.IO.Storage;
using Commodore.GameLogic.Network.Services;

namespace Commodore.GameLogic.Network
{
    public abstract class Device
    {
        public virtual string Address { get; protected set; } = "AAA:AA:AA:AAA";
        public Directory RootDirectory { get; protected set; } = new Directory("/", null);

        public Dictionary<ushort, Service> Services { get; protected set; }

        public AuthenticationService GetFirstActiveAuthenticationService()
            => (AuthenticationService)Services.Values.FirstOrDefault(x =>
                x is AuthenticationService serv && !serv.IsAuthenticated);
    }
}