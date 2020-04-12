using System;
using System.Collections.Generic;

namespace Commodore.GameLogic.World
{
    [Serializable]
    public class NetworkCursor
    {
        private Network Network { get; }

        public int X => CurrentConnectedDevice?.X ?? 0;
        public int Z => CurrentConnectedDevice?.Z ?? 0;

        public Device Gateway { get; set; }
        public Device CurrentConnectedDevice { get; set; }

        public NetworkCursor(Network network)
        {
            Network = network;
        }

        public void LinkTo(Device device)
        {
            if (CurrentConnectedDevice == null)
            {
                if (device != Gateway)
                    return;

                CurrentConnectedDevice = Gateway;
            }
            else
            {
                if (!CurrentConnectedDevice.LinkableNeighbours.Contains(device))
                    return;

                CurrentConnectedDevice = device;
            }

            if (IsNearNetworkBorder())
            {
                Network.Generate(X, Z);
            }
        }

        public void Unlink()
        {
            CurrentConnectedDevice = null;
        }

        private bool IsNearNetworkBorder()
        {
            return X >= Network.MaxX - 3 ||
                   X <= Network.MinX + 3 ||
                   Z >= Network.MaxZ - 3 ||
                   Z <= Network.MinZ + 3;
        }
    }
}
