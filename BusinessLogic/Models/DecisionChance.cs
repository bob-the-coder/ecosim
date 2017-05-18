using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;

namespace BusinessLogic.Models
{
    public class DecisionChance : ISinglePkModel
    {
        public int Id { get; set; }
        public int SimulationId { get; set; }
        public int DecisionId { get; set; }
        public double Chance { get; set; }
        public bool Enabled { get; set; }
    }
}
