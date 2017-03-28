namespace Models.Interfaces
{
    public interface IProduct : IIntPkObject
    {
        int SimulationId { get; set; }
        string Name { get; set; }
    }
}
