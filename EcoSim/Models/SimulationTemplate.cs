using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Configuration;
using BusinessLogic.Models;
using Models;

namespace EcoSim.Models
{
    public class SimulationTemplate
    {
        public SimulationSettings Simulation { get; set; }
        public NetworkConfiguration NetworkConfiguration { get; set; }
        public List<DecisionChance> DecisionChances { get; set; }
        public HttpPostedFileBase NetworkImport { get; set; }

        public double BaseMoney { get; set; }
        public int MoneyDeviation { get; set; }
    }
}