using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;

namespace BusinessLogic.Models
{
    public class SimulationLog : ISinglePkModel
    {
        public int Id { get; set; }
        public int SimulationId { get; set; }
        public int NodeId { get; set; }
        public int IterationNumber { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
    }
}
