using Models.Interfaces;

namespace Models
{
    public class NodeLink : INodeLink
    {
        public int NodeId { get; set; }
        public int LinkId { get; set; }
    }
}
