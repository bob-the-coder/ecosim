using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Helpers
{
    public abstract class StoredProcedureBase
    {
        #region Private Fields

        private StoredProcedures _StoredProcedure;

        private Dictionary<string, object> _Parameters;

        #endregion

        #region Properties

        public StoredProcedures StoredProcedure
        {
            get
            {
                return _StoredProcedure;
            }
            private set
            {
                _StoredProcedure = value;
            }
        }

        public Dictionary<string, object> Parameters
        {
            get
            {
                return _Parameters;
            }
            private set
            {
                _Parameters = value;
            }
        }

        #endregion

        #region Constructors

        public StoredProcedureBase(StoredProcedures storedProcedure)
        {
            this.StoredProcedure = storedProcedure;
            this.Parameters = new Dictionary<string, object>();
        }

        #endregion
    }
}
