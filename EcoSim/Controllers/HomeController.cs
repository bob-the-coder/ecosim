using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
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
        public async Task<JsonResult> GetNode()
        {
            var result = await NodeManager.GetAsync(new Guid("3B922936-3B22-411E-A069-3CC3B1B0F54B"));

            return Json(result);
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
        public async Task AddNodeToNetwork(List<Guid> linkIds)
        {
            var n = new Node { Id = Guid.NewGuid() };
            n = await NodeManager.CreateAsync(n);
            await NodeManager.AddLinks(n, linkIds);
        }

        [HttpPost]
        public async Task CreateInitialPopulation(int networkSize = 128, int linkPattern = 0)
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
            if (linkPattern == 0)
            {
                for (var i = 0; i < networkSize - 1; i++)
                {
                    links.Add(new NodeLink
                    {
                        NodeId = network[i].Id,
                        LinkId = network[i + 1].Id
                    });
                }
                links.Add(new NodeLink
                {
                    NodeId = network[networkSize - 1].Id,
                    LinkId = network[0].Id
                });
            }
            if (linkPattern == 1)
            {
                var centroid1 = new Random().Next(0, networkSize - 1);
                var centroid2 = new Random().Next(0, centroid1 - 1);
                for (var i = 0; i < networkSize; i++)
                {
                    if(i == centroid1 || i == centroid2) continue;
                    links.Add(new NodeLink
                    {
                        NodeId = network[centroid1].Id,
                        LinkId = network[i].Id
                    });
                    links.Add(new NodeLink
                    {
                        NodeId = network[i].Id,
                        LinkId = network[i + 1].Id
                    });
                    i++;
                    if (i == centroid1) i++;
                    if (i == centroid2) i++;
                    if (i == centroid1) i++;
                    links.Add(new NodeLink
                    {
                        NodeId = network[centroid2].Id,
                        LinkId = network[i].Id
                    });
                }
            }
            if (linkPattern == 2)
            {
                var prob = 80;
                for (var i = 0; i < networkSize; i++)
                {
                    for (var j = 0; j < networkSize; j++)
                    {
                        if(i == j || links.Any(
                            link => (link.NodeId == network[i].Id && link.LinkId == network[j].Id)
                                 || (link.NodeId == network[j].Id && link.LinkId == network[i].Id)))
                        {
                            continue;
                        }
                        var p = new Random().Next(1, 100);
                        if (p >= prob)
                        {
                            links.Add(new NodeLink
                            {
                                NodeId = network[i].Id,
                                LinkId = network[j].Id
                            });
                        }
                    }
                }
            }

            await NodeManager.NetworkInitialCreate(network, links).ConfigureAwait(false);
        }
    }
}