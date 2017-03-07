using DatabaseHandler.Helpers;
using Models;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeUpdate:StoredProcedureBase
    {
        public NodeUpdate(Node model) : base(StoredProcedures.NodeUpdate)
        {
            Parameters.Add("@Id", model.Id);
            Parameters.Add("@Name", model.Name);
            Parameters.Add("@SpendingLimit", model.SpendingLimit);
        }
    }
}
