using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Commodore.Framework;

namespace Commodore.GameLogic.Network
{
    [Serializable]
    public class Internet
    {
        [NonSerialized]
        public const int TickInterval = 50;

        public Dictionary<Address, Device> Devices { get; } = new Dictionary<Address, Device>();

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

        public Device GetDevice(Address address)
        {
            if (!Devices.ContainsKey(address))
                return null;

            return Devices[address];
        }

        public void AddDevice(Device device, Vector2? centerPoint = null, int? maxDistance = null)
        {
            if (device.Address == null)
            {
                device.Address = Address.Random();
            }

            if (Devices.ContainsKey(device.Address.Value))
                throw new InvalidOperationException("Network address collision detected.");

            if (centerPoint == null)
                centerPoint = Vector2.Zero;

            if (maxDistance == null)
                maxDistance = 5;

            var targetPosition = new Vector2(
                centerPoint.Value.X + G.Random.Next(1, maxDistance.Value),
                centerPoint.Value.Y + G.Random.Next(1, maxDistance.Value)
            );

            device.NetX = (int)targetPosition.X;
            device.NetY = (int)targetPosition.Y;
            
            Devices[device.Address.Value] = device;
        }

        public List<Device> GetDevicesInRange(Vector2 startingPoint, int range)
        {
            return Devices.Where(x => x.Value.GetDistanceTo(startingPoint) <= range)
                .Select(x => x.Value).ToList();
        }
    }
}