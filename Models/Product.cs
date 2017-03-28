using Models.Interfaces;

namespace Models
{
    public class Product : IProduct
    {
        public int Id { get; set; }
        public int SimulationId { get; set; }
        public string Name { get; set; }
    }
}
