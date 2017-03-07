using DatabaseHandler.Helpers;
using Models;

namespace DatabaseHandler.StoreProcedures
{
    public class ProductCreate : StoredProcedureBase
    {
        public ProductCreate(Product model) : base(StoredProcedures.ProductCreate)
        {
            Parameters.Add("@Name", model.Name);
        }
    }
}
