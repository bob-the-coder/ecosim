namespace DataLayer.Interfaces
{
    public interface ISinglePkDataAccessObject : IDataAccessObject
    {
        int Id { get; set; }
    }
}
