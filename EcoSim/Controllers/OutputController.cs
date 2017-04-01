using System.IO;
using System.Net;
using System.Web.Mvc;
using BusinessLogic;

namespace EcoSim.Controllers
{
    public class OutputController : Controller
    {
        [HttpPost]
        public JsonResult Simulate(int id, int numberOfIterations = 1)
        {
            for (var i = 0; i < numberOfIterations; i++)
            {
                Simulator.SimulateIteration(id);

                var thisIterationLogs = BaseCore.GetLogsInIteration(id, i);

                var physPath = HttpContext.Server.MapPath(Constants.LogDumpLocation);

                if (!Directory.Exists(physPath))
                {
                    Directory.CreateDirectory(physPath);
                }

                var simPath = physPath + "/Simulation " + id;

                if (!Directory.Exists(simPath))
                {
                    Directory.CreateDirectory(simPath);
                }

                var logPath = simPath + "/" + i + ".txt";

                using (var file = new StreamWriter(logPath))
                {
                    foreach (var log in thisIterationLogs)
                    {
                        file.WriteLine(log);
                    }
                }
            }

            return Json(true);
        }
    }
}