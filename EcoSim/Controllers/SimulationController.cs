using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Configuration;
using BusinessLogic.Cores;
using BusinessLogic.Enum;
using BusinessLogic.ModelCore;
using BusinessLogic.Models;
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
        public async Task<ActionResult> Create(SimulationTemplate template)
        {
            var simulation = await SimulationCore.CreateAsync(template.Simulation, true).ConfigureAwait(false);
            if (simulation == null)
            {
                return Json(new ServerResponse(true, "Failed to create Simulation"));
            }

            foreach (var decision in template.DecisionChances)
            {
                decision.SimulationId = simulation.Id;
                var lastDecision =
                    template.DecisionChances.LastOrDefault(d => d.DecisionId < decision.DecisionId && d.Enabled);
                if (lastDecision != null)
                {
                    decision.Chance += lastDecision.Chance;
                }
            }

            var savedDecisionChances = await DecisionChanceCore.CreateAsync(template.DecisionChances, true).ConfigureAwait(false);
            if (savedDecisionChances == null)
            {
                return Json(new ServerResponse(true, "Failed to create Decision Chances"));
            }

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
                        Id = i,
                        Name = $"{i}",
                        SimulationId = simulation.Id,
                        SpendingLimit = Rng.NextDouble() * Rng.Next(100, 500) * 10
                    });
                }
            }

            var savedNetwork = await NodeCore.CreateAsync(network, true).ConfigureAwait(false);
            if (savedNetwork == null || !savedNetwork.Any())
            {
                return Json(new ServerResponse(true, "Failed to create Node Network"));
            }

            var links = new List<NodeLink>();
            network = savedNetwork.ToList();

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
            var mirrorLinks = new List<NodeLink>();
            links.ForEach(nl =>
            {
                nl.SimulationId = simulation.Id;
                mirrorLinks.Add(new NodeLink
                {
                    SimulationId = simulation.Id,
                    NodeId = nl.LinkId,
                    LinkId = nl.NodeId
                });
            });
            links.AddRange(mirrorLinks);

            var savedLinks = await NodeLinkCore.CreateAsync(links, true).ConfigureAwait(false);
            if (savedLinks == null || !savedLinks.Any())
            {
                return Json(new ServerResponse(true, "Failed to create Network Links"));
            }

            var products = ProductManager.CreateProducts(
                (ProductCreationPattern)config.ProductCreationPattern, config.ProductBias, network.Count);

            foreach (var product in products)
            {
                product.SimulationId = simulation.Id;
            }

            var savedProducts = await ProductCore.CreateAsync(products, true).ConfigureAwait(false);
            if (savedProducts == null || !savedProducts.Any())
            {
                return Json(new ServerResponse(true, "Failed to create Products Set"));
            }

            products = savedProducts.ToList();

            var productions = await ProductManager.CreateProductions(
                network,
                products,
                (ProducerSelectionPattern)config.ProducerSelectionPattern,
                config.ProducerBias,
                (ProductionSelectionPattern)config.ProductionSelectionPattern,
                config.ProductionBias).ConfigureAwait(false);

            if (productions == null)
            {
                return Json(new ServerResponse(true, "Failed to create Productions Set"));
            }

            var needs = await ProductManager.CreateNeeds(
                network,
                products,
                productions,
                (NeedSelectionPattern)config.NeedSelectionPattern,
                config.NeedBias).ConfigureAwait(false);

            if (needs == null)
            {
                return Json(new ServerResponse(true, "Failed to create Needs Set"));
            }

            return Json(new ServerResponse(false, "Simulation successfully created"));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var simulations = await SimulationCore.GetIndexAsync().ConfigureAwait(false);

            return View(simulations);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            return View(id);
        }
    }
}