using System;
using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class SimSettingsGet : StoredProcedureBase
    {
        public SimSettingsGet(Guid id) : base(StoredProcedures.SimSettingsGet)
        {
            Parameters.Add("@SimulationId", id);
        }
    }
}
