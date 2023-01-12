using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Chroma.Graphics;
using Commodore.Engine;
using Commodore.Engine.Generators;
using Math = System.Math;

namespace Commodore.GameLogic.World
{
    [Serializable]
    public class Network
    {
        public const int DistanceBetweenDevices = 2;
        public const int GenerationRange = 16;

        public SimplexNoise Simplex { get; private set; }

        public NetworkCursor Cursor { get; private set; }
        public MersenneTwister Random { get; private set; }
        public List<Device> Devices { get; private set; }

        public bool DrawMap { get; set; }
        public int MapOffsetV { get; set; }
        public int MapOffsetH { get; set; }
        public int MapScale { get; set; }

        public int MinZ { get; set; }
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MaxZ { get; set; }

        public Network() : this((int)Time.Stamp) { }

        public Network(int seed)
        {
            DebugLog.Info($"Starting a new network with a seed of {seed}");

            Simplex = new SimplexNoise(seed);

            Random = new MersenneTwister(seed);
            Devices = new List<Device>();

            Cursor = new NetworkCursor(this);

            Generate(0, 0);

            var gatewayCandidate = FindRandomDevice(0);
            Cursor.Gateway = gatewayCandidate;
        }

        public void Generate(int startX, int startZ)
        {
            for (var x = startX - GenerationRange; x < startX + GenerationRange; x += DistanceBetweenDevices)
            {
                if (x > MaxX)
                    MaxX = x;
                else if (x < MaxX)
                    MaxX = x;

                for (var z = startZ - GenerationRange; z < startZ + GenerationRange; z += DistanceBetweenDevices)
                {
                    if (z > MaxZ)
                        MaxZ = z;
                    else if (z < MinZ)
                        MinZ = z;

                    var maxSecurityLevel = GetMaxSecurityLevel(x, z);

                    for (var securityLevel = 0; securityLevel <= 10; securityLevel++)
                    {
                        var noiseValue = Math.Abs(Simplex.Evaluate(x, securityLevel, z));
                        var actualSecurityLevel = Engine.Math.Clamp(securityLevel, 0, maxSecurityLevel);

                        if (noiseValue > 0.59 && !IsDeviceAt(x, z))
                        {
                            Devices.Add(
                                new Device(this, x, z, (int)actualSecurityLevel)
                            );

                            DebugLog.Info($"Device at: ({x}, {z}) -- security level {actualSecurityLevel}: {noiseValue}");
                        }
                    }
                }
            }

            foreach (var d in Devices)
            {
                d.LookForLinkableNeighbours();
            }

            // Devices.RemoveAll(x => !x.LinkableNeighbours.Any());
        }

        public Device FindRandomDevice(int maxSecurityLevel)
        {
            var meetingCriteria = Devices.Where(x => x.SecurityLevel <= maxSecurityLevel).ToList();
            return meetingCriteria[Random.Next(0, meetingCriteria.Count() - 1)];
        }

        public int GetMaxSecurityLevel(int x, int z)
        {
            var distanceBetween = Engine.Math.DistanceBetween(0, 0, x, z);

            if (distanceBetween >= 64)
                return 6;

            if (distanceBetween >= 128)
                return 8;

            if (distanceBetween >= 256)
                return 9;

            return 4;
        }

        public bool IsDeviceAt(int x, int z)
        {
            foreach (var device in Devices)
            {
                if (device.X == x && device.Z == z)
                    return true;
            }

            return false;
        }

        public Device DeviceAt(int x, int z)
        {
            foreach (var device in Devices)
            {
                if (device.X == x && device.Z == z)
                    return device;
            }

            return null;
        }

        public void Draw(RenderContext context)
        {
            try
            {
                for(var i = 0; i < Devices.Count; i++) 
                {
                    var d = Devices[i];

                    for(var j = 0; j < d.LinkableNeighbours.Count; j++) 
                    {
                        var d2 = d.LinkableNeighbours[j];

                        context.LineThickness = 1;
                        context.Line(
                            new Vector2(
                                MapOffsetH + (d.X * MapScale) + MapScale / 2,
                                MapOffsetV + (d.Z * MapScale) + MapScale / 2
                            ),
                            new Vector2(
                                MapOffsetH + (d2.X * MapScale) + MapScale / 2,
                                MapOffsetV + (d2.Z * MapScale) + MapScale / 2
                            ),
                            d.Color
                        );
                    }

                    d.Draw(context, MapOffsetH, MapOffsetV, MapScale);
                }
            }
            catch { }
        }
    }
}
