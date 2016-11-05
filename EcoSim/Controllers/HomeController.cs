using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Enum;
using BusinessLogic.Models;
using Newtonsoft.Json;

namespace EcoSim.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateNode(Node n)
        {
            n.Id = Guid.NewGuid();
            var result = await NodeManager.CreateAsync(n).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<string> GetGraph()
        {
            var result = await NodeManager.GetGraphAsync().ConfigureAwait(false);

            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public async Task CreateInitialPopulation(int networkSize = 128, int linkPattern = (int)PatternType.Circular)
        {
            var network = new List<Node>(networkSize);
            for (var i = 0; i < networkSize; i++)
            {
                network.Add(new Node
                {
                    Id = Guid.NewGuid(),
                    Name = $"Node {i}",
                    SpendingLimit = new Random().NextDouble() * new Random().Next(100, 500) * 10
                });
            }

            var links = new List<NodeLink>();

            switch ((PatternType)linkPattern)
            {
                case PatternType.Circular:
                    {
                        NetworkCreationPatterns.UsePatternCircular(networkSize, links, network);
                    }
                    break;
                case PatternType.Centroid:
                    {
                        NetworkCreationPatterns.UsePatternCentroid(networkSize, links, network);
                    }
                    break;
                case PatternType.Random:
                    {
                        NetworkCreationPatterns.UsePatternRandom(networkSize, links, network);
                    }
                    break;
                default:
                    return;
            }

            await NodeManager.NetworkInitialCreate(network, links).ConfigureAwait(false);
        }
    }
}