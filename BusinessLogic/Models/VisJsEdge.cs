using System;
using DatabaseHandler.Interfaces;

namespace BusinessLogic.Models
{
    public class VisJsEdge:IVisJsEdge
    {
        public Guid from { get; set; }
        public Guid to { get; set; }
    }
}
