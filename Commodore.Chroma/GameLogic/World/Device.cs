using Commodore.Engine;
using Chroma.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Commodore.GameLogic.World
{
    [Serializable]
    public class Device
    {
        public const int MaxLinkRange = 4;

        public Network Network { get; }
        public IP IP { get; }

        public Color Color
        {
            get
            {
                if (Network.Cursor.CurrentConnectedDevice == this)
                    return Color.White;

                switch (LinkableNeighbours.Count)
                {
                    case 1: return Color.Aqua;
                    case 2: return Color.LimeGreen;
                    case 3: return Color.Yellow;
                    case 4: return Color.Orange;
                    case 5: return Color.Magenta;
                    default: return Color.Red;
                }
            }
        }

        public int X { get; }
        public int Z { get; }

        public int SecurityLevel { get; }

        public List<Device> LinkableNeighbours { get; private set; }

        public Device(Network network, int x, int z, int securityLevel)
        {
            Network = network;
            IP = new IP(Network.Random);

            X = x;
            Z = z;

            LinkableNeighbours = new List<Device>();
            SecurityLevel = securityLevel;
        }

        public void Draw(RenderContext renderContext, int offsetH = 0, int offsetV = 0, int scale = 1)
        {
            renderContext.Rectangle(
                ShapeMode.Fill,
                new Vector2( 
                    offsetH + (X * scale),
                    offsetV + (Z * scale)
                ),
                scale, scale,
                Color
            );
        }

        public void LookForLinkableNeighbours()
        {
            var startX = X - MaxLinkRange;
            var startZ = Z - MaxLinkRange;

            for (var x = startX; x < X + MaxLinkRange; x++)
            {
                for (var z = startZ; z < Z + MaxLinkRange; z++)
                {
                    if (x == X && z == Z)
                        continue;

                    if (LinkableNeighbours.Count != 0)
                    {
                        if (LinkableNeighbours.Count >= 2)
                        {
                            continue;
                        }
                    }

                    var dev = Network.DeviceAt(x, z);

                    if (dev != null)
                    {
                        if (!dev.LinkableNeighbours.Contains(this) &&
                            !LinkableNeighbours.Contains(dev))
                        {
                            dev.LinkableNeighbours.Add(this);
                            LinkableNeighbours.Add(dev);
                        }
                    }
                }
            }
        }
    }
}
