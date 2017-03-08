using System;

namespace Models.Interfaces
{
    public interface IGuidPkObject : ISimObject
    {
        Guid Id { get; set; }
    }
}
