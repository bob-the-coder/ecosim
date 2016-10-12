using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeGetAll:StoredProcedureBase
    {
        public NodeGetAll() : base(StoredProcedures.NodeGetAll)
        {
        }
    }
}
