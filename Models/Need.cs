using Models.Interfaces;

namespace Models
{
    public class Need : INeed
    {
        public int NodeId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Priority { get; set; }
    }
}
