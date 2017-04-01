using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Configuration;
using BusinessLogic.Enum;
using EcoSim.Models;
using Models;
using Newtonsoft.Json;

namespace EcoSim.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Random Rng = new Random((int)DateTime.Now.ToBinary());
        public ActionResult Index()
        {
            return RedirectToAction("Create", "Simulation");
        }

        public ActionResult Simulations()
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
            return View("SimulationSettings", viewModel);
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
    }
}