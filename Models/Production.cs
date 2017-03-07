using Models.Interfaces;

namespace Models
{
    public class Production : IProduction
    {
        public int NodeId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Quality { get; set; }
        public double Price { get; set; }
    }
}
