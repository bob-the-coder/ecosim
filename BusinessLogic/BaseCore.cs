using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseHandler.Helpers;
using Models.Interfaces;

namespace BusinessLogic
{
    public class BaseCore
    {
        public static T Create<T>(T model, StoredProcedures procedure)
            where T : class, ISimObject, new()
        {
            var sp = new StoredProcedureBase(procedure, model);

            OperationStatus status;
            var result = StoredProcedureExecutor.GetSingleSetResult<T>(sp, out status);

            return status.Error ? null : result;
        }

        public static IList<T> Create<T>(IList<T> models, StoredProcedures procedure)
            where T : class, ISimObject, new()
        {
            var sps = models.Select(model => new StoredProcedureBase(procedure, model)).ToList();
            var result = new List<T>();

            OperationStatus status;
            foreach (var sp in sps)
            {
                var mResult = StoredProcedureExecutor.GetSingleSetResult<T>(sp, out status);
                if (status.Error)
                {
                    return null;
                }
                result.Add(mResult);
            }

            return result;
        }
    }
}
