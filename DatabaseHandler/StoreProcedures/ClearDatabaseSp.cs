using DatabaseHandler.Helpers;

namespace DatabaseHandler.StoreProcedures
{
    public class ClearDatabaseSp:StoredProcedureBase
    {
        public ClearDatabaseSp() : base(StoredProcedures.ClearDatabase) { }
    }
}
