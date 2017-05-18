using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;

namespace BusinessLogic.Models
{
    public class Product : ISinglePkModel
    {
        public int Id { get; set; }
        public int SimulationId { get; set; }
        public string Name { get; set; }
    }
}
