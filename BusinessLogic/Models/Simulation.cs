using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;

namespace BusinessLogic.Models
{
    public class Simulation : ISinglePkModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NeedFulfillSortByPriority { get; set; }
        public int NeedFulfillSortByQuantity { get; set; }
        public int ProductionSortByDistance { get; set; }
        public int ProductionSortByFinalCost { get; set; }
        public int ProductionSortByInitialCost { get; set; }
        public double ProductPriceIncreasePerQuality { get; set; }
        public double ProductPriceIncreasePerIntermediary { get; set; }
    }
}
