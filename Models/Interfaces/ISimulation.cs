namespace Models.Interfaces
{
    internal interface ISimulation : IIntPkObject
    {
        string Name { get; set; }
        int NeedFulfillSortByPriority { get; set; }
        int NeedFulfillSortByQuantity { get; set; }
        int ProductionSortByDistance { get; set; }
        int ProductionSortByFinalCost { get; set; }
        int ProductionSortByInitialCost { get; set; }
        double ProductPriceIncreasePerQuality { get; set; }
        double ProductPriceIncreasePerIntermediary { get; set; }
    }
}
