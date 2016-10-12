using System;

namespace DatabaseHandler.Interfaces
{
    public interface IVisJsEdge
    {
        Guid from { get; set; }
        Guid to { get; set; }
    }
}
