using DatabaseHandler.Helpers;
using Models;

namespace DatabaseHandler.StoreProcedures
{
    public class NeedUpdate : StoredProcedureBase
    {
        public NeedUpdate(Need model) : base(StoredProcedures.NeedUpdate)
        {
            Parameters.Add("@NodeId", model.NodeId);
            Parameters.Add("@ProductId", model.ProductId);
            Parameters.Add("@Quantity", model.Quantity);
            Parameters.Add("@Priority", model.Priority);
        }
    }
}
