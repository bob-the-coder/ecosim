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
        public ActionResult SearchResults(int id)
        {
            var log = new List<string>();
            Simulator.SimulateIteration(id);

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
        public JsonResult Simulate(int id, int numberOfIterations = 1)
        {
            var result = new List<IterationLog>();
            for (var i = 0; i < numberOfIterations; i++)
            {
                var log = new List<string>();
                Simulator.SimulateIteration(id);

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
            

            return View("SearchResults");
        }
    }
}