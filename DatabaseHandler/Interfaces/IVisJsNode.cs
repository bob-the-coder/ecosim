using System;

namespace DatabaseHandler.Interfaces
{
    public interface IVisJsNode
    {
        Guid id { get; set; }
        string label { get; set; }
        int group { get; set; }
        int value { get; set; }
    }
}
