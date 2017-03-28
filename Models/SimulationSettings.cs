using Models.Interfaces;

namespace Models
{
    public class SimulationSettings : ISimulation
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
        public int LatestIteration { get; set; }
        public int DecisionLookBack { get; set; }
    }
}
