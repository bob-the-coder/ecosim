using System;
using System.Collections.Generic;
using System.IO;
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
            if (simulation == null)
            {
                return Json(new ServerResponse(true, "Failed to create Simulation"));
            }

            var procedures = new List<StoredProcedureBase>();
            foreach (var decision in template.DecisionChances)
            {
                decision.SimulationId = simulation.Id;
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_Decision, decision));
            }
            if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                return Json(new ServerResponse(true, "Failed to create Decision Chances"));
            }
            procedures = new List<StoredProcedureBase>();

            var config = template.NetworkConfiguration;
            if (config.GridHeight != 0 && config.GridWidth != 0 && config.NetworkPattern == (int)NetworkPatternType.Grid)
            {
                config.NetworkSize = config.GridHeight * config.GridWidth;
            }

            var network = new List<Node>(config.NetworkSize);
            var gephiMinId = -1;
            var importMemStream = new MemoryStream();
            if (template.NetworkConfiguration.NetworkPattern == (int)NetworkPatternType.ImportGephi)
            {
                template.NetworkImport.InputStream.CopyTo(importMemStream);
                template.NetworkImport.InputStream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(template.NetworkImport.InputStream))
                {
                    var fileLine = streamReader.ReadLine();
                    if (string.IsNullOrEmpty(fileLine))
                    {
                        return Json(new ServerResponse(true, "Invalid input file"));
                    }
                    while (true)
                    {
                        fileLine = streamReader.ReadLine();
                        if (string.IsNullOrEmpty(fileLine))
                        {
                            return Json(new ServerResponse(true, "Encountered empty line before edge definition in input file"));
                        }

                        var indexOfEdgeDef = fileLine.IndexOf("edge", StringComparison.Ordinal);
                        if (indexOfEdgeDef == 0)
                        {
                            break;
                        }
                        var lineSplit = fileLine.Split(',');
                        var currentId = "";
                        try
                        {
                            currentId = lineSplit[0];
                            var id = int.Parse(currentId);
                            if (gephiMinId == -1)
                            {
                                gephiMinId = id;
                            }

                            network.Add(new Node
                            {
                                Id = id - gephiMinId,
                                SimulationId = simulation.Id,
                                Name = lineSplit[1],
                                SpendingLimit = Rng.NextDouble() * Rng.Next(100, 500) * 10
                            });
                        }
                        catch (Exception)
                        {
                            return Json(new ServerResponse(true, $"Failed to parse node Id (value {currentId}). Make sure Gephi Id's are int and first on node line"));
                        }
                    }
                }
            }
            else
            {
                for (var i = 0; i < config.NetworkSize; i++)
                {
                    network.Add(new Node
                    {
                        Name = $"{i}",
                        SimulationId = simulation.Id,
                        SpendingLimit = Rng.NextDouble() * Rng.Next(100, 500) * 10
                    });
                }
            }

            foreach (var node in network)
            {
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_Node, node));
            }
            if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                return Json(new ServerResponse(true, "Failed to create Node Network"));
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
                case NetworkPatternType.ImportGephi:
                    {
                        NetworkCreationPatterns.ImportFromGephi(links, network, importMemStream, gephiMinId);
                    }
                    break;
                default:
                    return Json(new ServerResponse(true, "Invalid network Pattern"));
            }

            if (links.Count == 0)
            {
                return Json(new ServerResponse(true, "Failed to define Network Links. Check input file or pattern parameters"));
            }

            foreach (var link in links)
            {
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_NodeLink, link));
            }
            if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                return Json(new ServerResponse(true, "Failed to create Network Links"));
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
                return Json(new ServerResponse(true, "Failed to create Products Set"));
            }
            products = BaseCore.GetFullSimulation(simulation.Id).Products;

            var productions = ProductManager.CreateProductions(
                network,
                products,
                (ProducerSelectionPattern)config.ProducerSelectionPattern,
                config.ProducerBias,
                (ProductionSelectionPattern)config.ProductionSelectionPattern,
                config.ProductionBias);
            if (productions == null)
            {
                return Json(new ServerResponse(true, "Failed to create Productions Set"));
            }

            var needs = ProductManager.CreateNeeds(
                network,
                products,
                productions,
                (NeedSelectionPattern)config.NeedSelectionPattern,
                config.NeedBias);

            if (needs == null)
            {
                return Json(new ServerResponse(true, "Failed to create Needs Set"));
            }

            return Json(new ServerResponse(false, "Simulation successfully created"));
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