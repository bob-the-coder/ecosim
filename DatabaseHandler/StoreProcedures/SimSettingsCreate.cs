using DatabaseHandler.Helpers;
using Models;

namespace DatabaseHandler.StoreProcedures
{
    public class SimSettingsCreate : StoredProcedureBase
    {
        public SimSettingsCreate(SimSettings model) : base(StoredProcedures.SimSettingsCreate)
        {
            Parameters.Add("@Id", model.Id);
            Parameters.Add("@NeedFulfillByPriority", model.NeedFulfillByPriority);
            Parameters.Add("@NeedFulfillByQuantity", model.NeedFulfillByQuantity);
            Parameters.Add("@ProductionSortByDistance", model.ProductionSortByDistance);
            Parameters.Add("@ProductionSortByFinalCost", model.ProductionSortByFinalCost);
            Parameters.Add("@ProductionSortByInitialCost", model.ProductionSortByInitialCost);
            Parameters.Add("@ProductPriceIncreasePerQuality", model.ProductPriceIncreasePerQuality);
            Parameters.Add("@ProductPriceIncreasePerIntermediary", model.ProductPriceIncreasePerIntermediary);
        }
    }
}
