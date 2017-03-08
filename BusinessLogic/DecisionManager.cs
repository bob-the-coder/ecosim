using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using BusinessLogic.Economics;
using Models;

namespace BusinessLogic
{
    public static class DecisionManager
    {
        private static readonly Random Rng = new Random((int)DateTime.Now.ToBinary());

        public static void Expand(this Node parentNode, ExpansionPattern pattern, int networkSize)
        {
            var childNode = new Node
            {
                Name = $"Node {networkSize + 1}",
                SpendingLimit = parentNode.SpendingLimit / pattern.WealthPercentage
            };

            parentNode.SpendingLimit -= parentNode.SpendingLimit / pattern.WealthPercentage;

            var childLinks = new List<NodeLink>();

            if (pattern.LinkToParent)
            {
                childLinks.Add(new NodeLink { NodeId = parentNode.Id, LinkId = childNode.Id });
            }

            if (pattern.AdditionalLinks > 0)
            {
                //TODO: add additional links
            }

            NodeManager.AppendToNetwork(parentNode.SimulationId, new List<Node> { childNode }, childLinks);
        }

        public static void ImproveProductionQuality(this Node node, List<Production> allProductions)
        {
            var ownProductions = allProductions.Where(p => p.NodeId == node.Id).ToList();

            foreach (var production in ownProductions)
            {
                var investmentCost = 250 * (100 + production.Quality * 2) / 100;
                if (node.SpendingLimit < investmentCost)
                {
                    continue;
                }

                production.Quality++;
                node.SpendingLimit -= investmentCost;
            }

            return;
        }

        public static void CreateProduction(this Node node, Simulation simSettings, List<Product> allProducts, List<Production> allProductions, List<Need> allNeeds)
        {
            var validProducts = allProducts.Where(
                product =>
                !allProductions.Any(p => p.ProductId == product.Id && p.NodeId == node.Id) &&
                !allNeeds.Any(n => n.ProductId == product.Id && n.NodeId == node.Id)).ToList();

            if (validProducts.Count == 0)
            {
                return;
            }

            var chosenProductIndex = Rng.Next(0, validProducts.Count - 1);

            var chosenProduct = allProducts[chosenProductIndex];

            var productionsForChosenProducts = allProductions.Where(p => p.ProductId == chosenProduct.Id).ToList();
            var needsForChosenProducts = allNeeds.Where(p => p.ProductId == chosenProduct.Id).ToList();

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
                chosenPrice = averagePrice * (1 - (double)neededQuantity / producedQuantity);
                chosenQuality = (int)(averageQuality * (1 - (double)neededQuantity / producedQuantity));

                if (neededQuantity != 0)
                {
                    chosenQuantity = Rng.Next(0, (int)(neededQuantity * (1 - (double)neededQuantity / producedQuantity)));
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

            var investmentCost = production.Quantity * production.PriceByQuality(simSettings);

            if (node.SpendingLimit < investmentCost)
            {
                return;
            }

            production = ProductionManager.Create(production);

            if (production == null)
            {
                return;
            }

            node.SpendingLimit -= investmentCost;
            allProductions.Add(production);
        }

        public static void CreateLink(this Node node, List<Node> network)
        {
            var validNodes = network.Where(n => n.Id != node.Id &&
            !node.Neighbours.Any(nb => nb.Id == n.Id)).ToList();

            if(validNodes.Count == 0)
            {
                return;
            }

            var targetIndex = Rng.Next(0, validNodes.Count - 1);
            var targetNode = validNodes[targetIndex];

            node.GetShortestPathsHeap(network);

            var pathToTarget = node.GetShortestPathToNode(targetNode, network);

            var investmentCost = 100.0;
            foreach(var interNode in pathToTarget)
            {
                investmentCost *= 1.3;
            }

            if(node.SpendingLimit < investmentCost)
            {
                return;
            }

            NodeManager.AddLinks(node, new List<int> { targetNode.Id });

            node.SpendingLimit -= investmentCost;
        }
    }
}
