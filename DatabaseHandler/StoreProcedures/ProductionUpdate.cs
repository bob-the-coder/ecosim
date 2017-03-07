using DatabaseHandler.Helpers;
using Models;

namespace DatabaseHandler.StoreProcedures
{
    public class ProductionUpdate:StoredProcedureBase
    {
        public ProductionUpdate(Production model) : base(StoredProcedures.ProductionUpdate)
        {
            Parameters.Add("@NodeId", model.NodeId);
            Parameters.Add("@ProductId", model.ProductId);
            Parameters.Add("@Price", model.Price);
            Parameters.Add("@Quality", model.Quality);
            Parameters.Add("@Quantity", model.Quantity);
        }
    }
}
