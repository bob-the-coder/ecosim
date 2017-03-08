using Models.Interfaces;

namespace Models
{
    public class DecisionChance : IDecisionChance
    {
        public int SimulationId { get; set; }
        public int DecisionId { get; set; }
        public double Chance { get; set; }
        public bool Enabled { get; set; }
    }
}
