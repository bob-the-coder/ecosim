using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using Models;

namespace BusinessLogic.Economics
{
    public static class TransactionManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buyer">Node making the purchase</param>
        /// <param name="buyerNeed">Buyer's Need regarding a product</param>
        /// <param name="seller">Node selling the product</param>
        /// <param name="sellerProduction">Seller's Production regarding a product</param>
        /// <param name="simSettings">Generic simulation settings</param>
        /// <param name="pathFromBuyerToSeller">Array of nodes between buyer and seller</param>
        /// <param name="log"></param>
        public static void BuysFrom(this Node buyer,
            Need buyerNeed,
            Node seller,
            Production sellerProduction,
            List<Node> pathFromBuyerToSeller,
            Simulation simSettings,
            List<string> log = null)
        {
            log?.Add($"Transaction between {buyer.Name} and {seller.Name}");

            var buyableQuantity = Math.Min(buyerNeed.Quantity, sellerProduction.Quantity);

            var pricePerInstance = sellerProduction.PriceByQualityAndDistance(simSettings, pathFromBuyerToSeller);

            var buyableQuantityPrice = buyableQuantity * pricePerInstance;

            var affordableQuantityPrice = Math.Min(buyer.SpendingLimit, buyableQuantityPrice);

            var affordableQuantity = (int)Math.Floor(affordableQuantityPrice / pricePerInstance);

            if (affordableQuantity == 0)
            {
                log?.Add($"Transaction aborted << {buyer.Name} has insufficient funds >>");
                log?.Add("");

                return;
            }

            var currentTransactionCost = affordableQuantityPrice;

            log?.Add($"{buyer.Name} pays {currentTransactionCost} for {affordableQuantity} x Pieces of Product");
            buyer.SpendingLimit -= currentTransactionCost;

            sellerProduction.Quantity -= affordableQuantity;
            buyerNeed.Quantity -= affordableQuantity;

            if (pathFromBuyerToSeller == null || pathFromBuyerToSeller.Count == 0)
            {
                return;
            }

            foreach (var intermediary in pathFromBuyerToSeller)
            {
                if (intermediary.Id == buyer.Id || intermediary.Id == seller.Id)
                {
                    continue;
                }

                var subTotal = currentTransactionCost/(1 + simSettings.ProductionSortByDistance);
                var nodeCostCut = subTotal * simSettings.ProductionSortByDistance;
                log?.Add($"{intermediary.Name} receives {nodeCostCut} for mediating transaction");

                intermediary.SpendingLimit += nodeCostCut;
                currentTransactionCost -= nodeCostCut;
            }

            log?.Add($"{seller.Name} receives {currentTransactionCost} for {affordableQuantity} x Pieces of Product");
            seller.SpendingLimit += currentTransactionCost;

            log?.Add("Transaction completed");
            log?.Add("");
        }
    }
}
