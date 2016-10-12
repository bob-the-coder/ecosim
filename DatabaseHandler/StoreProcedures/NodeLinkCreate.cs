using System;
using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class NodeLinkCreate:StoredProcedureBase
    {
        public NodeLinkCreate(Guid nodeId, Guid linkId) : base(StoredProcedures.NodeLinkCreate)
        {
            Parameters.Add("@NodeId", nodeId);
            Parameters.Add("@LinkId", linkId);
        }
    }
}
