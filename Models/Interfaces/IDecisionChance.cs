namespace Models.Interfaces
{
    public interface IDecisionChance : ISimObject
    {
        int SimulationId { get; set; }
        int DecisionId { get; set; }
        double Chance { get; set; }
        bool Enabled { get; set; }
    }
}
