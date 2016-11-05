using System;

namespace DatabaseHandler.Interfaces
{
    public interface INodeLink
    {
        Guid NodeId { get; set; }
        Guid LinkId { get; set; }
    }
}
