using DatabaseHandler.Helpers;
using Models;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeCreate:StoredProcedureBase
    {
        public NodeCreate(Node model) : base(StoredProcedures.NodeCreate)
        {
            Parameters.Add("Name", model.Name);
            Parameters.Add("SpendingLimit", model.SpendingLimit);
        }
    }
}
