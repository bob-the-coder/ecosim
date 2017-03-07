namespace DatabaseHandler.Interfaces
{
    public interface ISinglePkDataAccessObject : IDataAccessObject
    {
        int Id { get; set; }
    }
}