using Models.Interfaces;

namespace Models
{
    public class NodeLink : ISimObject
    {
        public int NodeId { get; set; }
        public int LinkId { get; set; }
    }
}
