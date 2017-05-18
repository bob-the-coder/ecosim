using BusinessLogic.ModelCore;
using BusinessLogic.Models;
using DataLayer.Implementation;

namespace BusinessLogic.Cores
{
    public class ProductionCore : BaseSinglePkCore<ProductionRepository, Production, DataLayer.Production>
    {
    }
}
