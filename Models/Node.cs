using Models.Interfaces;
using System.Collections.Generic;

namespace Models
{
    public class Node : INode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double SpendingLimit { get; set; }
        public virtual List<Node> Neighbours { get; set; }
        public virtual Dictionary<int, int> ShortestPathsHeap { get; set; }
    }
}
