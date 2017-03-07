using DatabaseHandler.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeLinkGetAll:StoredProcedureBase
    {
        public NodeLinkGetAll() : base(StoredProcedures.NodeLinkGetAll)
        {

        }
    }
}
