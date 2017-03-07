using DatabaseHandler.Helpers;
using Models;

namespace DatabaseHandler.StoreProcedures
{
    public class NeedCreate:StoredProcedureBase
    {
        public NeedCreate(Need model) : base(StoredProcedures.NeedCreate)
        {
            Parameters.Add("@NodeId", model.NodeId);
            Parameters.Add("@ProductId", model.ProductId);
            Parameters.Add("@Quantity", model.Quantity);
            Parameters.Add("@Priority", model.Priority);
        }
    }
}
