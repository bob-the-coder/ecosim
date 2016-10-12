using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Helpers
{
    public class OperationStatus
    {
        public bool Error { get; set; }
        public int ErrorCode { get; set; }
        public object ReturnValue { get; set; }

        public OperationStatus()
        {
            this.Error = true;
            this.ErrorCode = 0;
        }
    }
}
