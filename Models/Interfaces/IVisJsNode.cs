using System;

namespace Models.Interfaces
{
    public interface IVisJsNode : ISimObject
    {
        int id { get; set; }
        string label { get; set; }
        int group { get; set; }
        int value { get; set; }
    }
}
