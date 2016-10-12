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
            var n = new Node {Id = Guid.NewGuid()};
            n = await NodeManager.CreateAsync(n);
            await NodeManager.AddLinks(n, linkIds);
        }
    }
}