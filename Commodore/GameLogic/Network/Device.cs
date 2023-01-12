using System;
using System.Collections.Generic;
using System.Numerics;
using Chroma.Diagnostics.Logging;
using Commodore.GameLogic.Core;
using Commodore.GameLogic.Core.IO.Storage;

namespace Commodore.GameLogic.Network
{
    [Serializable]
    public abstract class Device : Entity
    {
        private Log _log => LogManager.GetForCurrentAssembly();

        protected Dictionary<byte, Entity> Entities { get; } = new Dictionary<byte, Entity>();

        public Address? Address { get; set; }

        public bool ShellEnabled { get; set; } = true;
        public Directory RootDirectory { get; } = new Directory("/", null);
        public SystemContext SystemContext { get; }

        public int NetX { get; set; }
        public int NetY { get; set; }

        public Device()
        {
            SystemContext = new SystemContext(this);
        }

        public virtual void OnShellAttached()
        {
            foreach (var neighbor in Entities)
                neighbor.Value.NeighborShellAttached(this);
        }

        public virtual void OnShellDetached()
        {
            foreach (var neighbor in Entities)
                neighbor.Value.NeighborShellDetached(this);
        }
        
        public virtual void OnLinked()
        {
        }

        public virtual void OnUnlinked()
        {
        }

        public void CreateServiceNode<T>(byte port) where T : Node
        {
            if (port == 0)
            {
                _log.Warning("Tried to create a service node at reserved port 0.");
                return;
            }

            if (Entities.ContainsKey(port))
            {
                _log.Warning($"Tried to create a service node at an occupied port {port}.");
                return;
            }

            Entities.Add(port, (T)Activator.CreateInstance(typeof(T), new object[] {this}));
        }

        public string GetHostName()
        {
            try
            {
                return RootDirectory.Subdirectory("etc")
                    .File("hostname")
                    .GetData();
            }
            catch
            {
                return Address?.ToString() ?? Network.Address.Zero.ToString();
            }
        }

        public bool IsConnectedToDevice(byte port)
            => Entities.ContainsKey(port);

        public bool HasServiceNode(byte port)
            => Entities.ContainsKey(port) && Entities[port] is Node;

        public Node GetServiceNode(byte port)
        {
            if (!HasServiceNode(port))
                return null;

            return Entities[port] as Node;
        }
        
        public Vector2 GetNetPositionVector()
            => new Vector2(NetX, NetY);

        public float GetDistanceTo(Vector2 point)
            => MathF.Abs(GetNetPositionVector().Length() - point.Length());
    }
}