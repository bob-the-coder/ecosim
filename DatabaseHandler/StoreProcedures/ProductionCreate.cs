using DatabaseHandler.Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseHandler.StoreProcedures
{
    public class ProductionCreate:StoredProcedureBase
    {
        public ProductionCreate(Production model) : base(StoredProcedures.ProductionCreate)
        {
            Parameters.Add("@NodeId", model.NodeId);
            Parameters.Add("@ProductId", model.ProductId);
            Parameters.Add("@Price", model.Price);
            Parameters.Add("@Quantity", model.Quantity);
            Parameters.Add("@Quality", model.Quality);
        }
    }
}
