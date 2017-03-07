using System;

namespace Models.Interfaces
{
    public interface IVisJsNode
    {
        int id { get; set; }
        string label { get; set; }
        int group { get; set; }
        int value { get; set; }
    }
}
