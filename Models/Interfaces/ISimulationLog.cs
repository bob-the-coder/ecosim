using System;

namespace Models.Interfaces
{
    public interface ISimulationLog : IGuidPkObject
    {
        int SimulationId { get; set; }
        int NodeId { get; set; }
        int IterationNumber { get; set; }
        int Type { get; set; }
        string Content { get; set; }
    }
}
