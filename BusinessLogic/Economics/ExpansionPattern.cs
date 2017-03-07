namespace BusinessLogic.Economics
{
    public class ExpansionPattern
    {
        public double WealthPercentage { get; set; }
        public bool LinkToParent { get; set; }
        public int AdditionalLinks { get; set; }
        public bool InheritNeeds { get; set; }
        public int AdditionalNeeds { get; set; }
        public bool InheritProduction { get; set; }
        public int AdditionalProductions { get; set; }
    }
}
