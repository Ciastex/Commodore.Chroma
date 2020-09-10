using System;
using System.Collections.Generic;

namespace Commodore.GameLogic.Network
{
    [Serializable]
    public class Internet
    {
        public Dictionary<Address, Device> Devices { get; } = new Dictionary<Address, Device>();

        public Internet()
        {
        }

        public Device GetDevice(string address)
        {
            var addr = Address.Parse(address);

            if (!Devices.ContainsKey(addr))
                return null;

            return Devices[addr];
        }

        public void AddDevice(Device device)
        {
            if (Devices.ContainsKey(device.Address))
                throw new InvalidOperationException("Network address collision detected.");

            Devices[device.Address] = device;
        }
    }
}