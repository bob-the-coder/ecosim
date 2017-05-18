using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Network Size")]
        public int NodeCount { get; set; }

        [Display(Name = "No. of Links")]
        public int LinkCount { get; set; }

        [Display(Name = "Product Pool Size")]
        public int ProductCount { get; set; }

        [Display(Name = "Production Pool Size")]
        public int ProductionCount { get; set; }

        [Display(Name = "Need Pool Size")]
        public int NeedCount { get; set; }

        [Display(Name = "Iterations Done")]
        public int IterationsDone { get; set; }
    }
}
