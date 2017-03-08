using System;

namespace Models.Interfaces
{
    public interface INeed : ISimObject
    {
        int NodeId { get; set; }
        int ProductId { get; set; }
        int Quantity { get; set; }
        int Priority { get; set; }
    }
}
