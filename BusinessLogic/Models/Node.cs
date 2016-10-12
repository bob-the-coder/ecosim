using System;
using DatabaseHandler.Interfaces;

namespace BusinessLogic.Models
{
    public class Node:INode
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double? SpendingLimit { get; set; }
    }
}
