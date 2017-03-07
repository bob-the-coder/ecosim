using System;
using Models.Interfaces;

namespace Models
{
    public class VisJsNode:IVisJsNode
    {
        public int id { get; set; }
        public string label { get; set; }
        public int group { get; set; }
        public int value { get; set; }
    }
}
