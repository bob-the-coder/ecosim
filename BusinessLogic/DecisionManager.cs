using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using BusinessLogic.Economics;
using BusinessLogic.Enum;
using DatabaseHandler.Helpers;
using Models;

namespace BusinessLogic
{
    public static class DecisionManager
    {
        private static readonly Random Rng = new Random((int)DateTime.Now.ToBinary());

        public static void Expand(this Node parentNode, FullSimulation sim, ExpansionPattern pattern)
        {
            var investmentCost = parentNode.SpendingLimit / pattern.WealthPercentage;

            var childNode = new Node
            {
                Id = -1,
                SimulationId = sim.Simulation.Id,
                Name = $"Node {sim.Network.Count + 1}",
                SpendingLimit = investmentCost
            };

            childNode = BaseCore.CreateNode(childNode);

            parentNode.SpendingLimit -= investmentCost;

            var childLinks = new List<NodeLink>();

            if (pattern.LinkToParent)
            {
                childLinks.Add(new NodeLink { NodeId = parentNode.Id, LinkId = childNode.Id });
            }

            if (pattern.AdditionalLinks > 0)
            {
                //TODO: add additional links
            }

            var procedures = new List<StoredProcedureBase>
            {
                new StoredProcedureBase(StoredProcedures.Save_Node, parentNode, ignore: Constants.NodeIgnoreNav)
            };
            foreach (var link in childLinks)
            {
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_NodeLink, link));
            }

            var success = StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures);
            if (!success)
            {
                return;
            }

            sim.CommitLog(new SimulationLog
            {
                Type = (int)SimulationLogType.Decision,
                NodeId = parentNode.Id,
                Content = $"{(int)Enum.Decision.Expand} {investmentCost}"
            });
        }

        public static void ImproveProductionQuality(this Node node, FullSimulation sim)
        {
            var ownProductions = sim.Productions.Where(p => p.NodeId == node.Id).ToList();

            foreach (var production in ownProductions)
            {
                var investmentCost = 250 * (100 + production.Quality * 2) / 100;
                if (node.SpendingLimit < investmentCost)
                {
                    continue;
                }

                production.Quality++;
                node.SpendingLimit -= investmentCost;

                var procedures = new List<StoredProcedureBase>
                {
                    new StoredProcedureBase(StoredProcedures.Save_Production, production),
                    new StoredProcedureBase(StoredProcedures.Save_Node, node,ignore: Constants.NodeIgnoreNav)
                };

                if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
                {
                    continue;
                }

                sim.CommitLog(new SimulationLog
                {
                    Type = (int)SimulationLogType.Decision,
                    NodeId = node.Id,
                    Content = $"{(int)Enum.Decision.ImproveProductions} {investmentCost}"
                });
            }
        }

        public static void CreateProduction(this Node node, FullSimulation currentSim)
        {
            var validProducts = currentSim.Products.Where(
                product =>
                !currentSim.Productions.Any(p => p.ProductId == product.Id && p.NodeId == node.Id) &&
                !currentSim.Needs.Any(n => n.ProductId == product.Id && n.NodeId == node.Id)).ToList();

            if (validProducts.Count == 0)
            {
                return;
            }

            var chosenProductIndex = Rng.Next(0, validProducts.Count - 1);

            var chosenProduct = currentSim.Products[chosenProductIndex];

            var productionsForChosenProducts = currentSim.Productions.Where(p => p.ProductId == chosenProduct.Id).ToList();
            var needsForChosenProducts = currentSim.Needs.Where(p => p.ProductId == chosenProduct.Id).ToList();

            var averagePrice = 10.0;
            if (productionsForChosenProducts.Count > 0)
            {
                averagePrice = productionsForChosenProducts.Average(p => p.Price);
            }

            var averageQuality = 20;
            if (productionsForChosenProducts.Count > 0)
            {
                averageQuality = (int)productionsForChosenProducts.Average(p => p.Quality);
            }

            var neededQuantity = 0;
            if (needsForChosenProducts.Count > 0)
            {
                neededQuantity = needsForChosenProducts.Sum(p2 => p2.Quantity);
            }

            var producedQuantity = 0;
            if (productionsForChosenProducts.Count > 0)
            {
                producedQuantity = productionsForChosenProducts.Sum(p2 => p2.Quantity);
            }

            var chosenPrice = averagePrice;
            var chosenQuality = averageQuality;
            var chosenQuantity = 30;

            if (producedQuantity != 0)
            {
                var generalRatio = Math.Sqrt(Math.Pow(1 - (double)neededQuantity / producedQuantity, 2));
                chosenPrice = averagePrice * generalRatio;
                chosenQuality = (int)(averageQuality * generalRatio);

                if (neededQuantity != 0)
                {
                    chosenQuantity = Rng.Next(0, (int)(neededQuantity * generalRatio));
                }
            }


            var production = new Production
            {
                NodeId = node.Id,
                ProductId = chosenProduct.Id,
                Price = chosenPrice,
                Quality = chosenQuality,
                Quantity = chosenQuantity
            };

            var investmentCost = production.Quantity * production.PriceByQuality(currentSim.Simulation);

            if (node.SpendingLimit < investmentCost)
            {
                return;
            }

            node.SpendingLimit -= investmentCost;

            var procedures = new List<StoredProcedureBase>
                {
                    new StoredProcedureBase(StoredProcedures.Save_Production, production),
                    new StoredProcedureBase(StoredProcedures.Save_Node, node, ignore: Constants.NodeIgnoreNav)
                };

            if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                return;
            }

            currentSim.CommitLog(new SimulationLog
            {
                Type = (int)SimulationLogType.Decision,
                NodeId = node.Id,
                Content = $"{(int)Enum.Decision.CreateProductions} {investmentCost}"
            });
        }

        public static void CreateLink(this Node node, FullSimulation currentSim)
        {
            var validNodes = currentSim.Network.Where(n => n.Id != node.Id &&
            node.Neighbours.All(nb => nb.Id != n.Id)).ToList();

            if (validNodes.Count == 0)
            {
                return;
            }

            var targetIndex = Rng.Next(0, validNodes.Count - 1);
            var targetNode = validNodes[targetIndex];

            node.GetShortestPathsHeap(currentSim.Network);

            var pathToTarget = node.GetShortestPathToNode(targetNode, currentSim.Network);

            var investmentCost = 100.0;
            foreach (var interNode in pathToTarget)
            {
                investmentCost *= 1.3;
            }

            if (node.SpendingLimit < investmentCost)
            {
                return;
            }

            node.SpendingLimit -= investmentCost;

            var procedures = new List<StoredProcedureBase>
                {
                    new StoredProcedureBase(StoredProcedures.Save_NodeLink, new NodeLink
                    {
                        NodeId = node.Id,
                        LinkId = targetNode.Id
                    }),
                    new StoredProcedureBase(StoredProcedures.Save_Node, node, ignore: Constants.NodeIgnoreNav)
                };

            if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                return;
            }

            currentSim.CommitLog(new SimulationLog
            {
                Type = (int)SimulationLogType.Decision,
                NodeId = node.Id,
                Content = $"{(int)Enum.Decision.CreateLinks} {investmentCost}"
            });
        }
    }
}
