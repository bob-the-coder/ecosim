using System;

namespace DatabaseHandler.Interfaces
{
    public interface INode
    {
        Guid Id { get; set; }
        string Name { get; set; }
        double? SpendingLimit { get; set; }
    }
}
