using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Models.Interfaces;

namespace DatabaseHandler.Helpers
{
    public class StoredProcedureBase
    {
        #region Private Fields

        #endregion

        #region Properties

        public StoredProcedures StoredProcedure { get; private set; }

        public Dictionary<string, object> Parameters { get; private set; }

        #endregion

        #region Constructors

        public StoredProcedureBase(StoredProcedures storedProcedure, ISimObject model = null, string where = null, string[] ignore = null)
        {
            StoredProcedure = storedProcedure;
            Parameters = new Dictionary<string, object>();

            if (model != null)
            {
                var memberNames = model.GetType().GetMembers()
                    .Where(m => m.MemberType == MemberTypes.Property)
                    .Select(m2 => m2.Name);
                foreach (var memberName in memberNames)
                {
                    Parameters.Add($"@{memberName}", model.GetType().GetProperty(memberName).GetValue(model));
                }
            }

            if (ignore?.Length > 0)
            {
                foreach (var ignored in ignore)
                {
                    Parameters[$"@{ignored}"] = null;
                }
            }

            if (!string.IsNullOrWhiteSpace(where))
            {
                Parameters.Add("@Where", where);
            }
        }

        #endregion
    }
}
