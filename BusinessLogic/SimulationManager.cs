using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using DatabaseHandler.Helpers;
using DatabaseHandler.StoreProcedures;
using Models;

namespace BusinessLogic
{
    public class SimulationManager
    {
        public static SimulationSettings GetSettings(Guid id)
        {
            var sp = new SimSettingsGet(id);
            OperationStatus status;
            var result = StoredProcedureExecutor.GetSingleSetResult<SimulationSettings>(sp, out status);

            return result;
        }

        public static SimulationSettings Create(SimulationSettings model)
        {
            model.Id = -1;
            var sp = new SimSettingsCreate(model);
            OperationStatus status;
            var result = StoredProcedureExecutor.GetSingleSetResult<SimulationSettings>(sp, out status);

            return result;
        }
    }
}
