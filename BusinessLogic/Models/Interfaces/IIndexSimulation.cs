using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Interfaces
{
    public interface IIndexSimulation
    {
        int Id { get; set; }
        string Name { get; set; }
        int NodeCount { get; set; }
        int LinkCount { get; set; }
        int ProductCount { get; set; }
        int ProductionCount { get; set; }
        int NeedCount { get; set; }
        int IterationsDone { get; set; }
    }
}
