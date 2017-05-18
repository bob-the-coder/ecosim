using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;

namespace BusinessLogic.Models
{
    public class NodeLink : ISinglePkModel
    {
        public int Id { get; set; }
        public int SimulationId { get; set; }
        public int NodeId { get; set; }
        public int LinkId { get; set; }
    }
}
