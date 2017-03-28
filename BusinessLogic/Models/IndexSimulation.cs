using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Interfaces;

namespace BusinessLogic.Models
{
    public class IndexSimulation : IIndexSimulation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NodeCount { get; set; }
        public int LinkCount { get; set; }
        public int ProductCount { get; set; }
        public int ProductionCount { get; set; }
        public int NeedCount { get; set; }
        public int IterationsDone { get; set; }
    }
}
