using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using WebGrease.Css.Extensions;
using System.IO;
using EcoSim.Models;

namespace EcoSim.Controllers
{
    public class OutputController : Controller
    {
        // GET: Output
        public ActionResult SearchResults(Guid id)
        {
            var log = new List<string>();
            Simulator.SimulateIteration(id, log);

            var fileName = $"Iteration-{DateTime.Now.ToBinary()}.txt";
            const string path = @"C:\SimOutput\";
            var filePath = $"{path}{fileName}";

            using (var outputFile = System.IO.File.AppendText(filePath))
            {
                foreach (var line in log)
                {
                    outputFile.WriteLine(line);
                }
            }

            return View(log);
        }

        [HttpPost]
        public JsonResult Simulate(Guid id, int numberOfIterations = 1)
        {
            var result = new List<IterationLog>();
            for (var i = 0; i < numberOfIterations; i++)
            {
                var log = new List<string>();
                Simulator.SimulateIteration(id, log);

                var iterationLog = new IterationLog
                {
                    Name = $"Iteration_{i + 1}_{DateTime.Now}",
                    Lines = log
                };

                result.Add(iterationLog);
            }

            return Json(result);
        }

        [HttpGet]
        public ActionResult TestDecision()
        {
            var productions = NodeManager.GetAllProductions();
            var prods = NodeManager.GetAllProducts();
            var needs = NodeManager.GetAllNeeds();
            var network = NodeManager.GetAll();
            var links = NodeManager.GetAllLinks();
            Simulator.LinkNodes(network, links);

            var node = network[0];

            node.CreateLink(network);

            Simulator.CommitChanges(network);

            return View("SearchResults");
        }
    }
}