using System;
using DatabaseHandler.Interfaces;

namespace BusinessLogic.Models
{
    public class NodeLink : INodeLink
    {
        public Guid NodeId { get; set; }
        public Guid LinkId { get; set; }
    }
}
