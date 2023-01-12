namespace Commodore.GameLogic.Network.Devices
{
    public class PersonalComputer : Device, INode
    {
        public NodeType Type => NodeType.Device;
        
        public bool IsActive { get; set; }
        public virtual string Banner => "coreOS_1.0 PCNODE";
        
        public byte[] GetResponse(byte[] data)
            => new byte[1] {0x00};
    }
}