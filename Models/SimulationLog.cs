using System;
using Models.Interfaces;

namespace Models
{
    public class SimulationLog : ISimulationLog
    {
        public Guid Id { get; set; }
        public int SimulationId { get; set; }
        public int NodeId { get; set; }
        public int IterationNumber { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
    }
}
