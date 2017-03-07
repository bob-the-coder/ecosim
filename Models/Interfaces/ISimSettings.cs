namespace Models.Interfaces
{
    internal interface ISimSettings
    {
        int Id { get; set; }
        int NeedFulfillByPriority { get; set; }
        int NeedFulfillByQuantity { get; set; }
        int ProductionSortByDistance { get; set; }
        int ProductionSortByFinalCost { get; set; }
        int ProductionSortByInitialCost { get; set; }
        double ProductPriceIncreasePerQuality { get; set; }
        double ProductPriceIncreasePerIntermediary { get; set; }
    }
}
