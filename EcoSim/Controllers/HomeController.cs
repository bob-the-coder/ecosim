using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Configuration;
using BusinessLogic.Enum;
using Models;
using Newtonsoft.Json;

namespace EcoSim.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Random Rng = new Random((int)DateTime.Now.ToBinary());
        public ActionResult Index()
        {
            return RedirectToAction("Simulations");
        }

        public ActionResult Simulations()
        {
            return View("SimulationSettings");
        }

        [HttpPost]
        public JsonResult CreateNode(Node n)
        {
            var result = NodeManager.Create(n);

            return Json(result);
        }

        [HttpPost]
        public string GetGraph()
        {
            var result = NodeManager.GetGraph();

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public void CreateInitialPopulation(NetworkConfiguration config)
        {
            if (config.GridHeight != 0 && config.GridWidth != 0 && config.NetworkPattern == (int)NetworkPatternType.Grid)
            {
                config.NetworkSize = config.GridHeight * config.GridWidth;
            }

            var network = new List<Node>(config.NetworkSize);
            for (var i = 0; i < config.NetworkSize; i++)
            {
                network.Add(new Node
                {
                    Name = $"{i}",
                    SpendingLimit = Rng.NextDouble() * Rng.Next(100, 500) * 10
                });
            }

            var dbIsClean = Simulator.ClearDatabase();

            if (!dbIsClean)
            {
                return;
            }

            network = NodeManager.AppendToNetwork(network);

            var links = new List<NodeLink>();

            switch ((NetworkPatternType)config.NetworkPattern)
            {
                case NetworkPatternType.Circular:
                    {
                        NetworkCreationPatterns.UsePatternCircular(config.NetworkSize, links, network);
                    }
                    break;
                case NetworkPatternType.Centroid:
                    {
                        NetworkCreationPatterns.UsePatternCentroid(config.NetworkSize, links, network);
                    }
                    break;
                case NetworkPatternType.Random:
                    {
                        NetworkCreationPatterns.UsePatternRandom(config.NetworkSize, links, network);
                    }
                    break;
                case NetworkPatternType.Grid:
                    {
                        NetworkCreationPatterns.UsePatternGrid(links, network, config.GridHeight, config.GridWidth);
                    }
                    break;
                case NetworkPatternType.SmallWorld:
                    {
                        NetworkCreationPatterns.UsePatternSmallWorld(config.NetworkSize, links, network, config.SwInitialDegree, config.SwRewireChance);
                    }
                    break;
                default:
                    return;
            }

            if (links.Count == 0)
            {
                return;
            }

            network = NodeManager.AppendToNetwork(links: links);

            if (network == null)
            {
                return;
            }

            var products = ProductManager.CommitProducts(
                (ProductCreationPattern)config.ProductCreationPattern, config.ProductBias, network.Count);

            var productions = ProductManager.CreateProductions(
                network,
                products,
                (ProducerSelectionPattern)config.ProducerSelectionPattern,
                config.ProducerBias,
                (ProductionSelectionPattern)config.ProductionSelectionPattern,
                config.ProductionBias);

            var needs = ProductManager.CreateNeeds(
                network,
                products,
                productions,
                (NeedSelectionPattern)config.NeedSelectionPattern,
                config.NeedBias);
        }
    }
}