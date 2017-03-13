using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Economics
{
    public class ExpansionPatterns
    {
        public static ExpansionPattern SimpleChild { get; } = new ExpansionPattern
        {
            WealthPercentage = 10,
            LinkToParent = true,
            AdditionalLinks = 0,
            InheritNeeds = true,
            AdditionalNeeds = 0,
            InheritProduction = true,
            AdditionalProductions = 0
        };
    }
}
