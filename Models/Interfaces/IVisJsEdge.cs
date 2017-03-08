using System;

namespace Models.Interfaces
{
    public interface IVisJsEdge : ISimObject
    {
        int from { get; set; }
        int to { get; set; }
    }
}
