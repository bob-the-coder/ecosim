using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using BusinessLogic.Economics;
using BusinessLogic.Enum;
using Models;
using DatabaseHandler.StoreProcedures;
using DatabaseHandler.Helpers;
using Decision = BusinessLogic.Enum.Decision;

namespace BusinessLogic
{
    public static class Simulator
    {
        private static readonly Random Rng = new Random();

        public static bool ClearDatabase()
        {
            var sp = new ClearDatabaseSp();
            return StoredProcedureExecutor.ExecuteNoQueryAsTransaction(new List<StoredProcedureBase> { sp });
        }

        public static bool CommitChanges(this FullSimulation currentSim)
        {
            var procedures = new List<StoredProcedureBase>();
            currentSim.Network.ForEach(node => procedures.Add(new NodeUpdate(node)));
            currentSim.Productions.ForEach(production => procedures.Add(new ProductionUpdate(production)));
            currentSim.Needs.ForEach(need => procedures.Add(new NeedUpdate(need)));

            return StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures);
        }

        public static void SimulateIteration(int id)
        {
            var currentSim = BaseCore.GetFullSimulation(id);

            LinkNodes(currentSim.Network, currentSim.Links);

            var log = new SimulationLog
            {
                Type = (int)SimulationLogType.GeneralInfo,
                NodeId = -99,
                Content = "TRANSACTION PHASE"
            };
            currentSim.CommitLog(log);

            foreach (var node in currentSim.Network)
            {
                var nodeNeeds = currentSim.Needs.Where(need => need.NodeId == node.Id).ToList();
                if (nodeNeeds.Count == 0)
                {
                    continue;
                }

                node.GetShortestPathsHeap(currentSim.Network);

                foreach (var need in nodeNeeds)
                {
                    var chanceToFulfill = Rng.Next(1, 100);

                    if (chanceToFulfill <= need.Priority)
                    {
                        currentSim.FulfillNeed(node, need, node.ShortestPathsHeap);
                    }
                }
            }

            var result = currentSim.CommitChanges();

            //DECISION PHASE
            currentSim.DecisionChances = currentSim.DecisionChances.OrderBy(d => d.Chance).ToList();

            foreach (var node in currentSim.Network)
            {
                var currentDecisionChance = Rng.NextDouble();
                for (var i = 0; i < currentSim.DecisionChances.Count; i++)
                {
                    if (!currentSim.DecisionChances[i].Enabled)
                    {
                        continue;
                    }
                    if (currentDecisionChance <= currentSim.DecisionChances[i].Chance)
                    {
                        switch ((Decision)currentSim.DecisionChances[i].DecisionId)
                        {
                            case Decision.Expand:
                                {
                                    node.Expand(ExpansionPatterns.SimpleChild, currentSim.Network.Count);
                                }
                                break;
                            case Decision.ImproveProductions:
                                {
                                    node.ImproveProductionQuality(currentSim.Productions);
                                }
                                break;
                            case Decision.CreateProductions:
                                {
                                    node.CreateProduction(currentSim);
                                }
                                break;
                            case Decision.CreateLinks:
                                {
                                    node.CreateLink(currentSim.Network);
                                }
                                break;
                        }
                        break;
                    }
                }
            }
        }

        public static void LinkNodes(List<Node> network, List<NodeLink> links)
        {
            foreach (var node in network)
            {
                var neighbours = new List<Node>();
                foreach (var link in links.Where(link => link.NodeId == node.Id))
                {
                    neighbours.Add(network.First(neighbour => neighbour.Id == link.LinkId).ShallowCopy());
                }

                node.Neighbours = neighbours;
            }
        }

        private static void FulfillNeed(this FullSimulation currentSim, Node node,
            Need need,
            Dictionary<int, int> nodeBfsResult)
        {
            var selectedProducer = node.GetClosestProducer(currentSim.Network, currentSim.Productions, need);

            if (selectedProducer == null)
            {
                return;
            }

            var selectedProduction =
                currentSim.Productions.First(p => p.NodeId == selectedProducer.Id && p.ProductId == need.ProductId);

            var pathToBuyer = node.GetShortestPathToNode(selectedProducer, currentSim.Network);

            node.BuysFrom(need, selectedProducer, selectedProduction, pathToBuyer.GetRange(1, pathToBuyer.Count - 1), currentSim);
        }

        private static Node GetClosestProducer(this Node buyer, List<Node> network, List<Production> allProductions, Need need)
        {
            var producers = network.Where(producer => allProductions.Any(production => production.NodeId == producer.Id && production.ProductId == need.ProductId)).ToList();

            if (producers.Count == 0)
            {
                return null;
            }

            var producerPaths = new Dictionary<Node, List<Node>>();

            foreach (var producer in producers)
            {
                var pathToProducer = buyer.GetShortestPathToNode(producer, network);

                if (pathToProducer == null || pathToProducer.Count == 0)
                {
                    continue;
                }
                producerPaths.Add(producer, pathToProducer);
            }

            if (producerPaths.Count == 0)
            {
                return null;
            }

            var shortestPathLength = producerPaths.Min(path => path.Value.Count);

            return producerPaths.First(path => path.Value.Count == shortestPathLength).Key;
        }
    }
}
