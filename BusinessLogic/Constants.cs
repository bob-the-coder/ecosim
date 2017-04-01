
namespace BusinessLogic
{
    public static class Constants
    {
        public const double Epsilon = 0.00001;
        public const int MaxImpact = 10000;
        public const string LogDumpLocation = "~/Logs";
        public static string[] NodeIgnoreNav => new[] { "Neighbours", "ShortestPathsHeap" };
    }
}
