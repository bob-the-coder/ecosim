using Models.Interfaces;

namespace Models
{
    public class Decision : IDecision
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
