using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeGet:StoredProcedureBase
    {
        public NodeGet(Guid id) : base(StoredProcedures.NodeGet)
        {
            Parameters.Add("@Id", id);
        }
    }
}
