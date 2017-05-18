using BusinessLogic.Enum;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Cores;
using BusinessLogic.Models;

namespace BusinessLogic
{
    public class ProductManager
    {
        private static readonly Random Rng = new Random();

        public static List<Product> CreateProducts(
            ProductCreationPattern pattern = ProductCreationPattern.Percentage,
            int presetValue = 50,
            int networkSize = 0)
        {
            if (presetValue < 0 || networkSize < 0)
            {
                return null;
            }

            var productsCount = 0;

            switch (pattern)
            {
                case ProductCreationPattern.Amount:
                    {
                        productsCount = presetValue;
                    }
                    break;
                case ProductCreationPattern.Percentage:
                    {
                        productsCount = presetValue * networkSize / 100;
                    }
                    break;
                default:
                    {
                        return null;
                    }
            }

            if (productsCount == 0)
            {
                return null;
            }

            var result = new List<Product>();
            for (var i = 0; i < productsCount; i++)
            {
                var product = new Product
                {
                    Name = $"Product {i + 1}"
                };

                result.Add(product);
            }

            return result;
        }

        public static async Task<List<Production>> CreateProductions(List<Node> network,
            List<Product> products,
            ProducerSelectionPattern selectionPattern = ProducerSelectionPattern.Percentage,
            int producerSelectionBias = 50,
            ProductionSelectionPattern productionPattern = ProductionSelectionPattern.Amount,
            int productionSelectionBias = 1)
        {
            List<Node> producers;
            switch (selectionPattern)
            {
                case ProducerSelectionPattern.Amount:
                    {
                        producers = ProducerManager.CreateByAmount(network, producerSelectionBias);
                    }
                    break;
                case ProducerSelectionPattern.Percentage:
                    {
                        producers = ProducerManager.CreateByNetworkPercentage(network, producerSelectionBias);
                    }
                    break;
                case ProducerSelectionPattern.ByChance:
                    {
                        producers = ProducerManager.CreateByChance(network, producerSelectionBias);
                    }
                    break;
                default:
                    {
                        return null;
                    }
            }

            return await CreateProductionsForNetworkSubset(producers, products, productionPattern, productionSelectionBias).ConfigureAwait(false);
        }

        private static async Task<List<Production>> CreateProductionsForNetworkSubset(List<Node> producers, List<Product> products, ProductionSelectionPattern pattern, int productionSelectionBias)
        {
            List<Production> productions;
            switch (pattern)
            {
                case (ProductionSelectionPattern.Amount):
                    {
                        productions = ProductionManager.CreateByAmount(producers, products, pattern, productionSelectionBias);
                    }
                    break;
                case ProductionSelectionPattern.Percentage:
                    {
                        productions = ProductionManager.CreateByProductPercentage(producers, products, pattern, productionSelectionBias);
                    }
                    break;
                case ProductionSelectionPattern.ByChance:
                    {
                        productions = ProductionManager.CreateByChance(producers, products, pattern, productionSelectionBias);
                    }
                    break;
                default:
                    {
                        return null;
                    }
            }
            return (await ProductionCore.CreateAsync(productions, true).ConfigureAwait(false)).ToList();
        }

        public static async Task<List<Need>> CreateNeeds(List<Node> network,
            List<Product> products,
            List<Production> productions,
            NeedSelectionPattern pattern = NeedSelectionPattern.SingleProduct,
            int needBias = 50)
        {
            var needs = new List<Need>();
            switch (pattern)
            {
                case (NeedSelectionPattern.SingleProduct):
                    {
                        foreach (var node in network)
                        {
                            var productIndex = Rng.Next(0, products.Count - 1);

                            var need = new Need
                            {
                                NodeId = node.Id,
                                ProductId = products[productIndex].Id,
                                Quantity = Rng.Next(1, 50),
                                Priority = Rng.Next(1, 100)
                            };

                            needs.Add(need);
                        }
                    }
                    break;
                case (NeedSelectionPattern.SingleFromProductions):
                    {
                        foreach (var node in network)
                        {
                            var productIndex = Rng.Next(0, productions.Count - 1);

                            var need = new Need
                            {
                                NodeId = node.Id,
                                ProductId = products[productIndex].Id,
                                Quantity = Rng.Next(1, 50),
                                Priority = Rng.Next(1, 100)
                            };

                            needs.Add(need);
                        }
                    }
                    break;
                case (NeedSelectionPattern.MoreProducts):
                    {
                        foreach (var node in network)
                        {
                            foreach (var product in products)
                            {
                                var prob = Rng.Next(0, 100);
                                if (prob > needBias)
                                {
                                    continue;
                                }

                                var need = new Need
                                {
                                    NodeId = node.Id,
                                    ProductId = product.Id,
                                    Quantity = Rng.Next(1, 50),
                                    Priority = Rng.Next(1, 100)
                                };

                                needs.Add(need);
                            }
                        }
                    }
                    break;
                case (NeedSelectionPattern.MoreFromProductions):
                    {
                        foreach (var node in network)
                        {
                            var producedProducts = products.Where(p => productions.Any(p2 => p2.ProductId == p.Id)).ToList();

                            foreach (var product in producedProducts)
                            {
                                var prob = Rng.Next(0, 100);
                                if (prob > needBias)
                                {
                                    continue;
                                }

                                var need = new Need
                                {
                                    NodeId = node.Id,
                                    ProductId = product.Id,
                                    Quantity = Rng.Next(1, 50),
                                    Priority = Rng.Next(1, 100)
                                };

                                needs.Add(need);
                            }
                        }
                    }
                    break;
                default:
                    {
                        return null;
                    }
            }

            return (await NeedCore.CreateAsync(needs, true).ConfigureAwait(false)).ToList();
        }
    }
}
