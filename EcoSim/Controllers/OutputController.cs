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

            var logPath = HttpContext.Server.MapPath(Constants.LogDumpLocation);
            for (var i = 0; i < numberOfIterations; i++)
            {
                Simulator.SimulateIteration(id, logPath);
            }

            return Json(true);
        }
    }
}