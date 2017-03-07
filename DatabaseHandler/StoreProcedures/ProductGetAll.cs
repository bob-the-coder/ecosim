using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class ProductGetAll:StoredProcedureBase
    {
        public ProductGetAll() : base(StoredProcedures.ProductGetAll) { }
    }
}
