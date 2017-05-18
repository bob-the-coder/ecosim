using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using BusinessLogic.Cores;
using BusinessLogic.Economics;
using BusinessLogic.Enum;
using BusinessLogic.ModelCore;
using BusinessLogic.Models;
using BusinessLogic.Workflow;
using Models;

namespace BusinessLogic
{
    public static class DecisionManager
    {
        private static readonly Random Rng = new Random((int)DateTime.Now.ToBinary());

        public static async Task<bool> Expand(this Node parentNode, FullSimulation sim, ExpansionPattern pattern)
        {
            var investmentCost = parentNode.SpendingLimit / pattern.WealthPercentage;

            var childNode = new Node
            {
                Id = -1,
                SimulationId = sim.Simulation.Id,
                Name = $"Node {sim.Network.Count + 1}",
                SpendingLimit = investmentCost
            };

            childNode = await NodeCore.CreateAsync(childNode).ConfigureAwait(false);
            if (childNode == null)
            {
                return false;
            }

            parentNode.SpendingLimit -= investmentCost;

            var childLinks = new List<NodeLink>();

            if (pattern.AdditionalLinks > 0)
            {
                //TODO: add additional links
            }

            parentNode = await NodeCore.UpdateAsync(parentNode, true).ConfigureAwait(false);
            if (parentNode == null)
            {
                return false;
            }

            if (pattern.LinkToParent)
            {
                childLinks.Add(new NodeLink { NodeId = parentNode.Id, LinkId = childNode.Id });
                childLinks.Add(new NodeLink { NodeId = childNode.Id, LinkId = parentNode.Id });

                var createdLinks = await NodeLinkCore.CreateAsync(childLinks, true).ConfigureAwait(false);
                if (createdLinks == null)
                {
                    return false;
                }
            }

            if (pattern.InheritNeeds)
            {
                var newNeeds = sim.Needs.Where(need => need.NodeId == parentNode.Id).ToList();
                newNeeds.ForEach(need => need.NodeId = childNode.Id);

                var createdNeeds = await NeedCore.CreateAsync(newNeeds, true).ConfigureAwait(false);
                if (createdNeeds == null)
                {
                    return false;
                }
            }

            if (pattern.InheritProduction)
            {
                var newProductions = sim.Productions.Where(prod => prod.NodeId == parentNode.Id).ToList();
                newProductions.ForEach(prod => prod.NodeId = childNode.Id);

                var createdProductions = await ProductionCore.CreateAsync(newProductions, true).ConfigureAwait(false);
                if (createdProductions == null)
                {
                    return false;
                }
            }

            var logEntry = await SimulationLogCore.CreateAsync(new SimulationLog
            {
                Type = (int)SimulationLogType.Decision,
                NodeId = parentNode.Id,
                Content = $"{(int)Enum.Decision.Expand} {investmentCost}"
            }).ConfigureAwait(false);

            return logEntry != null;
        }

        public static async Task<bool> ImproveProductionQuality(this Node node, FullSimulation sim)
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

                var savedProduction = await ProductionCore.UpdateAsync(production, true).ConfigureAwait(false);
                if (savedProduction == null)
                {
                    return false;
                }

                var savedNode = await NodeCore.UpdateAsync(node, true).ConfigureAwait(false);
                if (savedNode == null)
                {
                    return false;
                }
                savedNode.Neighbours = node.Neighbours;
                savedNode.ShortestPathsHeap = node.ShortestPathsHeap;
                node = savedNode;

                var logEntry = await SimulationLogCore.CreateAsync(new SimulationLog
                {
                    Type = (int)SimulationLogType.Decision,
                    NodeId = node.Id,
                    Content = $"{(int)Enum.Decision.ImproveProductions} {investmentCost}"
                }).ConfigureAwait(false);

                if (logEntry == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static async Task<bool> CreateProduction(this Node node, FullSimulation currentSim)
        {
            var validProducts = currentSim.Products.Where(
                product =>
                !currentSim.Productions.Any(p => p.ProductId == product.Id && p.NodeId == node.Id) &&
                !currentSim.Needs.Any(n => n.ProductId == product.Id && n.NodeId == node.Id)).ToList();

            if (validProducts.Count == 0)
            {
                return false;
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
                return false;
            }

            node.SpendingLimit -= investmentCost;

            var createdProduction = await ProductionCore.CreateAsync(production, true).ConfigureAwait(false);
            if (createdProduction == null)
            {
                return false;
            }

            var savedNode = await NodeCore.UpdateAsync(node, true).ConfigureAwait(false);
            if (savedNode == null)
            {
                return false;
            }

            savedNode.Neighbours = node.Neighbours;
            savedNode.ShortestPathsHeap = node.ShortestPathsHeap;
            node = savedNode;

            var logEntry = await SimulationLogCore.CreateAsync(new SimulationLog
            {
                Type = (int) SimulationLogType.Decision,
                NodeId = node.Id,
                Content = $"{(int) Enum.Decision.CreateProductions} {investmentCost}"
            }).ConfigureAwait(false);

            return logEntry != null;
        }

        public static async Task<bool> CreateLink(this Node node, FullSimulation currentSim)
        {
            var validNodes = currentSim.Network.Where(n => n.Id != node.Id &&
            node.Neighbours.All(nb => nb.Id != n.Id)).ToList();

            if (validNodes.Count == 0)
            {
                return false;
            }

            var targetIndex = Rng.Next(0, validNodes.Count - 1);
            var targetNode = validNodes[targetIndex];

            node.GetShortestPathsHeap(currentSim.Network);

            var pathToTarget = node.GetShortestPathToNode(targetNode, currentSim.Network);

            var investmentCost = 100.0;
            if (pathToTarget == null)
            {
                investmentCost *= Math.Pow(1.3, node.Neighbours.Count);
            }
            else
            {
                investmentCost *= Math.Pow(1.3, pathToTarget.Count);
            }

            if (node.SpendingLimit < investmentCost)
            {
                return false;
            }

            node.SpendingLimit -= investmentCost;

            var newLinks = new List<NodeLink>();
            newLinks.Add(new NodeLink { NodeId = node.Id, LinkId = targetNode.Id });
            newLinks.Add(new NodeLink { NodeId = targetNode.Id, LinkId = node.Id });

            var createdLinks = await NodeLinkCore.CreateAsync(newLinks, true).ConfigureAwait(false);
            if (createdLinks == null)
            {
                return false;
            }

            var savedNode = await NodeCore.UpdateAsync(node, true).ConfigureAwait(false);
            if (savedNode == null)
            {
                return false;
            }

            var logEntry = await SimulationLogCore.CreateAsync(new SimulationLog
            {
                Type = (int)SimulationLogType.Decision,
                NodeId = node.Id,
                Content = $"{(int)Enum.Decision.CreateLinks} {investmentCost}"
            }).ConfigureAwait(false);

            if (logEntry == null)
            {
                return false;
            }

            savedNode.Neighbours = node.Neighbours;
            savedNode.ShortestPathsHeap = node.ShortestPathsHeap;
            node = savedNode;
            return true;
        }
    }
}
