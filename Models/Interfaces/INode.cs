using System;

namespace Models.Interfaces
{
    public interface INode
    {
        int Id { get; set; }
        string Name { get; set; }
        double SpendingLimit { get; set; }
    }
}
