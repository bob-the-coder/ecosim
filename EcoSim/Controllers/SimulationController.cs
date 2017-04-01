using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Configuration;
using BusinessLogic.Enum;
using DatabaseHandler.Helpers;
using DatabaseHandler.StoreProcedures;
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
                Simulation = new SimulationSettings(),
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
        public ActionResult Create(SimulationTemplate template)
        {
            var simulation = BaseCore.Save(template.Simulation, StoredProcedures.SimSettingsCreate);

            var procedures = new List<StoredProcedureBase>();
            foreach (var decision in template.DecisionChances)
            {
                decision.SimulationId = simulation.Id;
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_Decision, decision));
            }
            if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                return Json("fail");
            }
            procedures = new List<StoredProcedureBase>();

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

            foreach (var node in network)
            {
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_Node, node));
            }
            if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                return Json("fail");
            }
            procedures = new List<StoredProcedureBase>();

            network = BaseCore.GetFullSimulation(simulation.Id).Network;

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

            foreach (var link in links)
            {
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_NodeLink, link));
            }
            if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                return Json("fail");
            }
            procedures = new List<StoredProcedureBase>();

            if (network == null)
            {
                return Json("Fail");
            }

            var products = ProductManager.CreateProducts(
                (ProductCreationPattern)config.ProductCreationPattern, config.ProductBias, network.Count);

            foreach (var product in products)
            {
                product.SimulationId = simulation.Id;
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_Product, product));
            }
            if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                return Json("fail");
            }
            products = BaseCore.GetFullSimulation(simulation.Id).Products;

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

        [HttpGet]
        public ActionResult Index()
        {
            var simulations = BaseCore.GetAllSimulations();

            return View(simulations);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(id);
        }
    }
}