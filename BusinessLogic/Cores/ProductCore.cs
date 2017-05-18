using BusinessLogic.ModelCore;
using BusinessLogic.Models;
using DataLayer.Implementation;

namespace BusinessLogic.Cores
{
    public class ProductCore : BaseSinglePkCore<ProductRepository, Product, DataLayer.Product>
    {
    }
}
