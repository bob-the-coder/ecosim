using System;

namespace Models
{
    public class SimSettings
    {
        public Guid Id { get; set; }
        public int NeedFulfillByPriority { get; set; }
        public int NeedFulfillByQuantity { get; set; }
        public int ProductionSortByDistance { get; set; }
        public int ProductionSortByFinalCost { get; set; }
        public int ProductionSortByInitialCost { get; set; }
        public double ProductPriceIncreasePerQuality { get; set; }
        public double ProductPriceIncreasePerIntermediary { get; set; }
    }
}
