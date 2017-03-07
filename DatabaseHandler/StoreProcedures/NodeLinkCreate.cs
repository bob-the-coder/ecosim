using System;
using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeLinkCreate : StoredProcedureBase
    {
        public NodeLinkCreate(int nodeId, int linkId) : base(StoredProcedures.NodeLinkCreate)
        {
            Parameters.Add("@NodeId", nodeId);
            Parameters.Add("@LinkId", linkId);
        }
    }
}
