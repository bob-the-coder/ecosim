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
        [HttpPost]
        public JsonResult Simulate(int id, int numberOfIterations = 1)
        {
            for (var i = 0; i < numberOfIterations; i++)
            {
                Simulator.SimulateIteration(id);
            }

            return Json(true);
        }
    }
}