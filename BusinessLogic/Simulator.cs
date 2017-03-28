using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
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

        public static bool SaveCurrentState(this FullSimulation sim)
        {
            var procedures = new List<StoredProcedureBase>();

            for (var i = 0; i < sim.Network.Count; i++)
            {
                if (procedures.Count / 100 == 1)
                {
                    if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
                    {
                        return false;
                    }
                    procedures = new List<StoredProcedureBase>();
                }
                sim.Network[i].Neighbours = null;
                sim.Network[i].ShortestPathsHeap = null;
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_Node, sim.Network[i]));
            }
            if (procedures.Count > 0)
            {
                if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
                {
                    return false;
                }
                procedures = new List<StoredProcedureBase>();
            }

            for (var i = 0; i < sim.Productions.Count; i++)
            {
                if (procedures.Count / 100 == 1)
                {
                    if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
                    {
                        return false;
                    }
                    procedures = new List<StoredProcedureBase>();
                }
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_Production, sim.Productions[i]));
            }
            if (procedures.Count > 0)
            {
                if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
                {
                    return false;
                }
                procedures = new List<StoredProcedureBase>();
            }

            for (var i = 0; i < sim.Needs.Count; i++)
            {
                if (procedures.Count / 100 == 1)
                {
                    if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
                    {
                        return false;
                    }
                    procedures = new List<StoredProcedureBase>();
                }
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_Need, sim.Needs[i]));
            }
            if (procedures.Count > 0)
            {
                if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
                {
                    return false;
                }
            }
            return true;
        }

        public static void SimulateIteration(int id)
        {
            var currentSim = BaseCore.GetFullSimulation(id);

            LinkNodes(currentSim.Network, currentSim.Links);

            var iterationStarted = BaseCore.StartIteration(currentSim.Simulation.Id);

            if (!iterationStarted)
            {
                return;
            }
            currentSim.Simulation.LatestIteration++;

            currentSim.CommitLog(new SimulationLog
            {
                NodeId = -99,
                Type = (int)SimulationLogType.GeneralInfo,
                Content = $"START ITERATION {currentSim.Simulation.LatestIteration}"
            });

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

            var result = currentSim.SaveCurrentState();
            if (!result)
            {
                return;
            }

            currentSim = BaseCore.GetFullSimulation(currentSim.Simulation.Id);
            LinkNodes(currentSim.Network, currentSim.Links);

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
                                    node.Expand(currentSim, ExpansionPatterns.SimpleChild);
                                }
                                break;
                            case Decision.ImproveProductions:
                                {
                                    node.ImproveProductionQuality(currentSim);
                                }
                                break;
                            case Decision.CreateProductions:
                                {
                                    node.CreateProduction(currentSim);
                                }
                                break;
                            case Decision.CreateLinks:
                                {
                                    node.CreateLink(currentSim);
                                }
                                break;
                        }
                        break;
                    }
                }
            }

            currentSim.NormalizeDecisions();

            currentSim.CommitLog(new SimulationLog
            {
                NodeId = -99,
                Type = (int)SimulationLogType.GeneralInfo,
                Content = $"END ITERATION {currentSim.Simulation.LatestIteration}"
            });
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

        private static void NormalizeDecisions(this FullSimulation simulation)
        {
            var decisions = simulation.DecisionChances;

            var lastIterationsLogs = BaseCore.GetMemberList<SimulationLog>(
                new SimulationMember { SimulationId = simulation.Simulation.Id },
                StoredProcedures.FullSimulation_GetSimulationLogs,
                $"IterationNumber >= {simulation.Simulation.LatestIteration - simulation.Simulation.DecisionLookBack}");

            var decisionLogs = lastIterationsLogs.Where(log => log.Type == (int)SimulationLogType.Decision).ToList();
            var transactionLogs = lastIterationsLogs.Where(log => log.Type == (int)SimulationLogType.Transaction).ToList();

            var sellerProfits = 0.0;
            var mediatorProfits = 0.0;
            var cummulativeProductionQuality = 0;

            for (var i = 0; i < transactionLogs.Count; i++)
            {
                var log = transactionLogs[i];

                var type = -1;
                int.TryParse(log.Content.Substring(0, 1), out type);

                if (type == (int)TransactionType.Sells && decisions[(int)Decision.CreateProductions].Enabled)
                {
                    sellerProfits += double.Parse(log.Content.Substring(3));
                }
                else if (type == (int)TransactionType.Mediates && decisions[(int)Decision.CreateLinks].Enabled)
                {
                    mediatorProfits += double.Parse(log.Content.Substring(3));
                }

                if (log.Type == (int)SimulationLogType.BoughtProduction)
                {
                    cummulativeProductionQuality += int.Parse(log.Content);
                }
            }

            var decisionPopularity = new[] { 0, 0, 0, 0 };
            for (var i = 0; i < decisionLogs.Count; i++)
            {
                var decisionType = int.Parse(decisionLogs[i].Content.Substring(0, 1));
                decisionPopularity[decisionType]++;
            }

            // calculate for each decision type, its impact
            var indexExpand = (int)Decision.Expand;
            var indexProduction = (int)Decision.CreateProductions;
            var indexQuality = (int)Decision.ImproveProductions;
            var indexMediate = (int)Decision.CreateLinks;

            var decisionImpacts = new[] { 0, 0, 0, 0 };
            decisionImpacts[indexExpand]
                = ImpactOfDecision((double)Constants.MaxImpact / 4, decisions[indexExpand].Chance, decisionPopularity[indexExpand]);

            decisionImpacts[indexMediate]
                = ImpactOfDecision(mediatorProfits, decisions[indexMediate].Chance, decisionPopularity[indexMediate]);

            decisionImpacts[indexProduction]
                = ImpactOfDecision(sellerProfits, decisions[indexProduction].Chance, decisionPopularity[indexProduction]);

            decisionImpacts[indexQuality]
                = ImpactOfDecision(cummulativeProductionQuality, decisions[indexQuality].Chance, decisionPopularity[indexQuality]);

            var fullImpact = decisionImpacts[0] + decisionImpacts[1] + decisionImpacts[2] + decisionImpacts[3];

            decisions[indexExpand].Chance = (double)decisionImpacts[indexExpand] / fullImpact;
            decisions[indexMediate].Chance = (double)decisionImpacts[indexMediate] / fullImpact;
            decisions[indexProduction].Chance = (double)decisionImpacts[indexProduction] / fullImpact;
            decisions[indexQuality].Chance = (double)decisionImpacts[indexQuality] / fullImpact;

            var procedures = new List<StoredProcedureBase>();
            foreach (var decision in decisions)
            {
                procedures.Add(new StoredProcedureBase(StoredProcedures.Save_Decision, decision));
            }

            if (StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures))
            {
                foreach (var decision in decisions)
                {
                    simulation.CommitLog(new SimulationLog
                    {
                        NodeId = -99,
                        Type = (int)SimulationLogType.DecisionNormalization,
                        Content = $"{decision.DecisionId} {decision.Chance} {decisionPopularity[decision.DecisionId]}"
                    });
                }
            }
        }

        private static int ImpactOfDecision(double score, double currentChance, double decisionPopularity)
        {
            if (currentChance < Constants.Epsilon)
            {
                return 0;
            }

            var partialScore = (int)(score * currentChance * decisionPopularity);

            return Math.Max(partialScore, Constants.MaxImpact);
        }
    }
}
