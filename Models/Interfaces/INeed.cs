using System;

namespace Models.Interfaces
{
    public interface INeed
    {
        int NodeId { get; set; }
        int ProductId { get; set; }
        int Quantity { get; set; }
        int Priority { get; set; }
    }
}
