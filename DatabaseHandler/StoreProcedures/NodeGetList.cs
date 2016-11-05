using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeGetList:StoredProcedureBase
    {
        public NodeGetList(string paramName, object paramValue):base(StoredProcedures.NodeGetList)
        {
            Parameters.Add($"@{paramName}", paramValue);
        }
    }
}
