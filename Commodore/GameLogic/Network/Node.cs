using System;

namespace Commodore.GameLogic.Network
{
    [Serializable]
    public abstract class Node : Entity
    {
        public virtual string Banner { get; }
        public bool IsActive { get; set; } = true;
        
        public Device Owner { get; }
        
        public Node(Device owner)
            => Owner = owner;

        public virtual void OnBound()
        {
        }

        public virtual void OnUnbound()
        {
        }

        public virtual byte[] GetResponse(byte[] data)
            => new byte[] {0x4C, 0x4F, 0x4C};

        protected byte[] Data(params byte[] bytes)
            => bytes;
    }
}