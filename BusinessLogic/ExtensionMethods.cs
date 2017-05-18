using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using BusinessLogic.Models;
using Models;

namespace BusinessLogic
{
    public static class ExtensionMethods
    {
        public static Node ShallowCopy(this Node original)
        {
            return new Node
            {
                Id = original.Id,
                Name = original.Name,
                SpendingLimit = original.SpendingLimit
            };
        }

        public static double PriceByQuality(this Production p, SimulationSettings simSettings)
        {
            return p.Price * Math.Pow(1 + simSettings.ProductPriceIncreasePerQuality, (double)p.Quality / 10);
        }

        public static double PriceByDistance(this Production p, SimulationSettings simSettings,
            List<Node> pathFromBuyerToSeller)
        {
            return p.Price * Math.Pow(1 + simSettings.ProductPriceIncreasePerIntermediary, pathFromBuyerToSeller.Count - 2);
        }

        public static double PriceByQualityAndDistance(this Production p, SimulationSettings simSettings,
            List<Node> pathFromBuyerToSeller)
        {
            return p.PriceByQuality(simSettings) * Math.Pow(1 + simSettings.ProductPriceIncreasePerIntermediary, pathFromBuyerToSeller.Count - 2);
        }
    }
}
