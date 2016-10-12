using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeCreate:StoredProcedureBase
    {
        public NodeCreate(dynamic model) : base(StoredProcedures.NodeCreate)
        {
            Parameters.Add("Id", model.Id);
            Parameters.Add("Name", model.Name);
            Parameters.Add("SpendingLimit", model.SpendingLimit);
        }
    }
}
