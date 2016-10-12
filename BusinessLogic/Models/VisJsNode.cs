using System;
using DatabaseHandler.Interfaces;

namespace BusinessLogic.Models
{
    public class VisJsNode:IVisJsNode
    {
        public Guid id { get; set; }
        public string label { get; set; }
        public int group { get; set; }
        public int value { get; set; }
    }
}
