using System;

namespace Models.Interfaces
{
    public interface INode : IIntPkObject
    {
        int SimulationId { get; set; }
        string Name { get; set; }
        double SpendingLimit { get; set; }
    }
}
