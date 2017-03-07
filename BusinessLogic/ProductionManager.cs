using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Enum;
using DatabaseHandler.Helpers;
using DatabaseHandler.StoreProcedures;
using Models;

namespace BusinessLogic
{
    public class ProductionManager
    {
        private static readonly Random Rng = new Random();

        public static List<Production> CommitProductions(List<Production> productions)
        {
            if (productions.Count == 0)
            {
                return null;
            }
            var procedures = new List<StoredProcedureBase>();
            foreach (var production in productions)
            {
                procedures.Add(new ProductionCreate(production));
            }

            return StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures) ? productions : null;
        }

        public static Production Create(Production production)
        {
            OperationStatus os;
            return StoredProcedureExecutor.GetSingleSetResult<Production>(new ProductionCreate(production), out os);
        }

        public static List<Production> CreateByAmount(List<Node> producers, List<Product> products, ProductionSelectionPattern pattern, int productionSelectionBias)
        {
            var productions = new List<Production>();

            productionSelectionBias = productionSelectionBias % products.Count;

            foreach (var node in producers)
            {
                var bias = productionSelectionBias;
                while (bias > 0)
                {
                    var productIndex = Rng.Next(0, products.Count - 1);

                    var production = new Production
                    {
                        NodeId = node.Id,
                        ProductId = products[productIndex].Id,
                        Price = Rng.NextDouble() * 100,
                        Quality = Rng.Next(1, 100),
                        Quantity = Rng.Next(1, 200)
                    };

                    if (productions.Any(p => p.NodeId == production.NodeId && p.ProductId == production.ProductId))
                    {
                        continue;
                    }

                    productions.Add(production);
                    bias--;
                }
            }

            return productions;
        }

        public static List<Production> CreateByProductPercentage(List<Node> producers, List<Product> products, ProductionSelectionPattern pattern, int productionSelectionBias)
        {
            var productions = new List<Production>();

            productionSelectionBias = (products.Count * productionSelectionBias / 100) % products.Count;

            foreach (var node in producers)
            {
                var bias = productionSelectionBias % products.Count;
                while (bias > 0)
                {
                    var productIndex = Rng.Next(0, products.Count - 1);

                    var production = new Production
                    {
                        NodeId = node.Id,
                        ProductId = products[productIndex].Id,
                        Price = Rng.NextDouble() * 100,
                        Quality = Rng.Next(1, 100),
                        Quantity = Rng.Next(1, 200)
                    };

                    if (productions.Any(p => p.NodeId == production.NodeId && p.ProductId == production.ProductId))
                    {
                        continue;
                    }

                    productions.Add(production);
                    bias--;
                }
            }

            return productions;
        }

        public static List<Production> CreateByChance(List<Node> producers, List<Product> products, ProductionSelectionPattern pattern, int productionSelectionBias)
        {
            var productions = new List<Production>();

            productionSelectionBias = productionSelectionBias % 100;

            foreach (var node in producers)
            {
                foreach (var product in products)
                {
                    var prob = Rng.Next(0, 100);

                    if (prob > productionSelectionBias)
                    {
                        continue;
                    }
                    var production = new Production
                    {
                        NodeId = node.Id,
                        ProductId = product.Id,
                        Price = Rng.NextDouble() * 100,
                        Quality = Rng.Next(1, 100),
                        Quantity = Rng.Next(1, 200)
                    };

                    productions.Add(production);
                }
            }

            return productions;
        }
    }
}
