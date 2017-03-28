using System.Collections.Generic;

namespace Models
{
    public class FullSimulation
    {
        public SimulationSettings Simulation { get; set; }
        public List<Node> Network { get; set; }
        public List<Product> Products { get; set; }
        public List<NodeLink> Links { get; set; }
        public List<Production> Productions { get; set; }
        public List<Need> Needs { get; set; }
        public List<DecisionChance> DecisionChances { get; set; }
        public List<SimulationLog> Logs { get; set; }
    }
}
