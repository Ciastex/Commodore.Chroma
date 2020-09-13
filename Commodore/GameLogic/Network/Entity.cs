using System;
using System.Threading.Tasks;

namespace Commodore.GameLogic.Network
{
    [Serializable]
    public class Entity
    {
        public virtual async Task Tick()
        {
            await Task.CompletedTask;
        }

        public virtual void NeighborShellDetached(Device device)
        {
        }

        public virtual void NeighborShellAttached(Device device)
        {
        }
    }
}