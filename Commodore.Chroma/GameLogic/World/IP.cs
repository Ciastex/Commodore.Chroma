using Commodore.Engine.Generators;
using System;

namespace Commodore.GameLogic.World
{
    [Serializable]
    public class IP
    {
        public string Address { get; private set; }

        public IP(MersenneTwister random)
        {
            Address = $"{random.Next(0, 4095):X3}:{random.Next(0, 4095):X3}:{random.Next(0, 4095):X3}";
        }
    }
}
