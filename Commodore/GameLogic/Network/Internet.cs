using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Commodore.GameLogic.Network
{
    [Serializable]
    public class Internet
    {
        public const int TickInterval = 50;
        
        public Dictionary<Address, Device> Devices { get; } = new Dictionary<Address, Device>();

        public Internet()
        {
        }

        public async Task Tick()
        {
            foreach (var device in Devices.Values)
                await device.Tick();

            await Task.Delay(TickInterval);
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