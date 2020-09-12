namespace Commodore.GameLogic.Network.Nodes
{
    public class SimpleAuthNode : INode
    {
        public string Banner => "SIMPLE_AUTH 1.0";
        public NodeType Type => NodeType.Authentication;
        
        public bool IsActive { get; set; }

        public SimpleAuthNode(Device device)
        {
            
        }

        public byte[] GetResponse(byte[] data)
        {
            throw new System.NotImplementedException();
        }
    }
}