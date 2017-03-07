using System;
using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeGet:StoredProcedureBase
    {
        public NodeGet(int id) : base(StoredProcedures.NodeGet)
        {
            Parameters.Add("@Id", id);
        }
    }
}
