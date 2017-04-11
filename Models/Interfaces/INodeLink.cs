using System;

namespace Models.Interfaces
{
    public interface INodeLink : IIntPkObject
    {
        int NodeId { get; set; }
        int LinkId { get; set; }
        int SimulationId { get; set; }
    }
}
