namespace Commodore.GameLogic.Network
{
    public abstract class Service
    {
        public bool IsActive { get; protected set; } = true;
        
        public abstract string Banner { get; }
        public abstract byte[] GetResponse(byte[] data);
    }
}