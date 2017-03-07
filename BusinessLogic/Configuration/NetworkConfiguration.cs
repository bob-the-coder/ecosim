namespace BusinessLogic.Configuration
{
    public class NetworkConfiguration
    {
        public int NetworkPattern { get; set; }
        public int NetworkSize { get; set; }
        public int GridHeight { get; set; }
        public int GridWidth { get; set; }
        public int SwInitialDegree { get; set; }
        public int SwRewireChance { get; set; }

        public int ProductCreationPattern { get; set; }
        public int ProductBias { get; set; }

        public int ProducerSelectionPattern { get; set; }
        public int ProducerBias { get; set; }

        public int ProductionSelectionPattern { get; set; }
        public int ProductionBias { get; set; }

        public int NeedSelectionPattern { get; set; }
        public int NeedBias { get; set; }
    }
}
