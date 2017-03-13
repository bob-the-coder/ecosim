using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using BusinessLogic.Enum;
using DatabaseHandler.Helpers;
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
        /// <param name="pathFromBuyerToSeller">Array of nodes between buyer and seller</param>
        /// <param name="currentSim">current simulation</param>
        public static void BuysFrom(this Node buyer,
            Need buyerNeed,
            Node seller,
            Production sellerProduction,
            List<Node> pathFromBuyerToSeller,
            FullSimulation currentSim)
        {
            currentSim.CommitLog(new SimulationLog
            {
                Type = (int)SimulationLogType.GeneralInfo,
                Content = "Transaction started"
            });

            var buyableQuantity = Math.Min(buyerNeed.Quantity, sellerProduction.Quantity);

            var pricePerInstance = sellerProduction.PriceByQualityAndDistance(currentSim.Simulation, pathFromBuyerToSeller);

            var buyableQuantityPrice = buyableQuantity * pricePerInstance;

            var affordableQuantityPrice = Math.Min(buyer.SpendingLimit, buyableQuantityPrice);

            var affordableQuantity = (int)Math.Floor(affordableQuantityPrice / pricePerInstance);

            if (affordableQuantity == 0)
            {
                return;
            }

            var currentTransactionCost = affordableQuantityPrice;

            var log = new SimulationLog
            {
                Type = (int)SimulationLogType.Transaction,
                NodeId = buyer.Id,
                Content = $"{(int)TransactionType.Buys} -{currentTransactionCost}"
            };
            currentSim.CommitLog(log);

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

                var subTotal = currentTransactionCost / (1 + currentSim.Simulation.ProductPriceIncreasePerIntermediary);
                var nodeCostCut = subTotal * currentSim.Simulation.ProductPriceIncreasePerIntermediary;

                log = new SimulationLog
                {
                    Type = (int)SimulationLogType.Transaction,
                    NodeId = intermediary.Id,
                    Content = $"{(int)TransactionType.Mediates} +{nodeCostCut}"
                };
                currentSim.CommitLog(log);

                intermediary.SpendingLimit += nodeCostCut;
                currentTransactionCost -= nodeCostCut;
            }

            log = new SimulationLog
            {
                Type = (int)SimulationLogType.Transaction,
                NodeId = seller.Id,
                Content = $"{(int)TransactionType.Sells} +{currentTransactionCost}"
            };
            currentSim.CommitLog(log);

            seller.SpendingLimit += currentTransactionCost;

            log = new SimulationLog
            {
                Type = (int)SimulationLogType.GeneralInfo,
                NodeId = seller.Id,
                Content = "Transaction completed"
            };
            currentSim.CommitLog(log);
        }

        public static SimulationLog CommitLog(this FullSimulation currentSim, SimulationLog log)
        {
            log.Id = Guid.NewGuid();
            log.SimulationId = currentSim.Simulation.Id;
            currentSim.Logs.Add(log);
            BaseCore.Create(log, StoredProcedures.SessionLogCreate);
            return log;
        }
    }
}
