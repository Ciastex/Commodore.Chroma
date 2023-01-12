using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chroma.Diagnostics.Logging;
using Commodore.GameLogic.Core.IO.Storage;

namespace Commodore.GameLogic.Network
{
    public abstract class Device
    {
        private Log _log => LogManager.GetForCurrentAssembly();

        protected Dictionary<byte, INode> Nodes { get; } = new Dictionary<byte, INode>();

        public Address Address { get; set; }

        public bool ShellEnabled { get; protected set; } = true;
        public Directory RootDirectory { get; } = new Directory("/", null);

        public virtual async Task Tick()
        {
        }

        public void CreateNode<T>(byte nodePort) where T : INode
        {
            if (nodePort == 0)
            {
                _log.Warning("Tried to create a node at reserved port 0.");
                return;
            }

            if (Nodes.ContainsKey(nodePort))
            {
                _log.Warning($"Tried to create a node at an occupied port {nodePort}.");
                return;
            }

            Nodes.Add(nodePort, (T)Activator.CreateInstance(typeof(T), new object[] {this}));
        }
    }
}