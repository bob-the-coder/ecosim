using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Configuration;
using BusinessLogic.Enum;
using DatabaseHandler.Helpers;
using EcoSim.Models;
using Models;

namespace EcoSim.Controllers
{
    public class SimulationController : Controller
    {
        private static readonly Random Rng = new Random((int)DateTime.Now.ToBinary());

        public ActionResult Create()
        {
            var viewModel = new SimulationTemplate
            {
                Simulation = new Simulation(),
                NetworkConfiguration = new NetworkConfiguration(),
                DecisionChances = new List<DecisionChance>()
            };
            for (var i = 0; i < 4; i++)
            {
                viewModel.DecisionChances.Add(new DecisionChance
                {
                    DecisionId = i,
                    Enabled = true,
                    Chance = 0.25
                });
            }
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Create(SimulationTemplate template)
        {
            var simulation = BaseCore.Create(template.Simulation, StoredProcedures.SimSettingsCreate);

            template.DecisionChances.ForEach(d => d.SimulationId = simulation.Id);
            var decisionChances = BaseCore.Create<DecisionChance>(template.DecisionChances, StoredProcedures.DecisionChanceCreate);

            var config = template.NetworkConfiguration;
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
                    SimulationId = simulation.Id,
                    SpendingLimit = Rng.NextDouble() * Rng.Next(100, 500) * 10
                });
            }

            network = NodeManager.AppendToNetwork(simulation.Id, network);

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
                    return Json("Fail");
            }

            if (links.Count == 0)
            {
                return Json("Fail");
            }

            network = NodeManager.AppendToNetwork(simulation.Id, links: links);

            if (network == null)
            {
                return Json("Fail");
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

            return Json("Success");
        }
    }
}