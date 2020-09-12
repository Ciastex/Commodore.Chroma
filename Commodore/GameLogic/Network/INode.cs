namespace Commodore.GameLogic.Network
{
    public interface INode
    {
        NodeType Type { get; }
        string Banner { get; }
        
        bool IsActive { get; set; }
        
        byte[] GetResponse(byte[] data);
    }
}