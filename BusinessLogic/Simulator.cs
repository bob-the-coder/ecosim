using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using BusinessLogic.Economics;
using BusinessLogic.Enum;
using Models;
using Decision = BusinessLogic.Enum.Decision;
using System.Web;
using BusinessLogic.Cores;
using BusinessLogic.ModelCore;
using BusinessLogic.Models;

namespace BusinessLogic
{
    public static class Simulator
    {
        private static readonly Random Rng = new Random();

        public static async Task<bool> SaveCurrentState(this FullSimulation sim)
        {
            var savedNodes = await NodeCore.UpdateAsync(sim.Network, true).ConfigureAwait(false);
            if (savedNodes == null) return false;

            var savedProductions = await ProductionCore.UpdateAsync(sim.Productions, true).ConfigureAwait(false);
            if (savedProductions == null) return false;

            var savedNeeds = await NeedCore.UpdateAsync(sim.Needs, true).ConfigureAwait(false);
            if (savedNeeds == null) return false;

            return true;
        }

        public static async Task<bool> SimulateIteration(int id, string logPath)
        {
            var currentSim = await SimulationCore.GetFullSimulationAsync(id).ConfigureAwait(false);

            LinkNodes(currentSim.Network, currentSim.Links);

            currentSim.Simulation.LatestIteration++;

            await currentSim.CommitLog(new SimulationLog
            {
                NodeId = -99,
                Type = (int)SimulationLogType.GeneralInfo,
                Content = $"START ITERATION {currentSim.Simulation.LatestIteration}"
            }).ConfigureAwait(false);

            var log = new SimulationLog
            {
                Type = (int)SimulationLogType.GeneralInfo,
                NodeId = -99,
                Content = "TRANSACTION PHASE"
            };
            await currentSim.CommitLog(log).ConfigureAwait(false);

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
                        await currentSim.FulfillNeed(node, need, node.ShortestPathsHeap).ConfigureAwait(false);
                    }
                }
            }

            var result = await currentSim.SaveCurrentState().ConfigureAwait(false);
            if (!result)
            {
                return false;
            }

            currentSim = await SimulationCore.GetFullSimulationAsync(id).ConfigureAwait(false);
            LinkNodes(currentSim.Network, currentSim.Links);

            //DECISION PHASE
            log = new SimulationLog
            {
                Type = (int)SimulationLogType.GeneralInfo,
                NodeId = -99,
                Content = "DECISION PHASE"
            };
            await currentSim.CommitLog(log).ConfigureAwait(false);

            currentSim.DecisionChances = currentSim.DecisionChances.OrderBy(d => d.Chance).ToList();

            foreach (var node in currentSim.Network)
            {
                var currentDecisionChance = Rng.NextDouble();

                var selectedDecision = currentSim.DecisionChances.FirstOrDefault(
                    decision => decision.Enabled && currentDecisionChance <= decision.Chance);

                if (selectedDecision == null)
                {
                    continue;
                }
                switch ((Decision)selectedDecision.DecisionId)
                {
                    case Decision.Expand:
                        {
                            await node.Expand(currentSim, ExpansionPatterns.SimpleChild).ConfigureAwait(false);
                        }
                        break;
                    case Decision.ImproveProductions:
                        {
                            await node.ImproveProductionQuality(currentSim).ConfigureAwait(false);
                        }
                        break;
                    case Decision.CreateProductions:
                        {
                            await node.CreateProduction(currentSim).ConfigureAwait(false);
                        }
                        break;
                    case Decision.CreateLinks:
                        {
                            await node.CreateLink(currentSim).ConfigureAwait(false);
                        }
                        break;
                }
            }

            await currentSim.NormalizeDecisions().ConfigureAwait(false);

            await currentSim.CommitLog(new SimulationLog
            {
                NodeId = -99,
                Type = (int)SimulationLogType.GeneralInfo,
                Content = $"END ITERATION {currentSim.Simulation.LatestIteration}"
            }).ConfigureAwait(false);

            await CreateLogFile(logPath, id, currentSim.Simulation.LatestIteration).ConfigureAwait(false);

            return true;
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

        private static async Task FulfillNeed(this FullSimulation currentSim, Node node,
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

            await node.BuysFrom(need, selectedProducer, selectedProduction, pathToBuyer.GetRange(1, pathToBuyer.Count - 1), currentSim).ConfigureAwait(false);
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

        private static async Task NormalizeDecisions(this FullSimulation simulation)
        {
            var decisions = simulation.DecisionChances;

            var lastIterationLogs =
                await SimulationLogCore.GetListAsync(sl => sl.SimulationId == simulation.Simulation.Id &&
                                                           sl.IterationNumber >= simulation.Simulation.LatestIteration - simulation.Simulation.DecisionLookBack)
                                                           .ConfigureAwait(false);

            var orderedLogs = lastIterationLogs.OrderBy(sl => sl.IterationNumber).ToList();

            var decisionLogs = orderedLogs.Where(log => log.Type == (int)SimulationLogType.Decision).ToList();
            var transactionLogs = orderedLogs.Where(log => log.Type == (int)SimulationLogType.Transaction).ToList();

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

            decisions[indexQuality].Chance = (double)decisionImpacts[indexQuality] / fullImpact;
            if (decisions[indexQuality].Chance > 0)
            {
                decisions[indexQuality].Chance += decisions[indexExpand].Chance;
            }

            decisions[indexProduction].Chance = (double)decisionImpacts[indexProduction] / fullImpact;
            if (decisions[indexProduction].Chance > 0)
            {
                if (decisions[indexQuality].Chance > 0)
                {
                    decisions[indexProduction].Chance += decisions[indexQuality].Chance;
                }
                else if (decisions[indexExpand].Chance > 0)
                {
                    decisions[indexProduction].Chance += decisions[indexExpand].Chance;
                }
            }

            decisions[indexMediate].Chance = (double)decisionImpacts[indexMediate] / fullImpact;
            if (decisions[indexMediate].Chance > 0)
            {
                if (decisions[indexProduction].Chance > 0)
                {
                    decisions[indexMediate].Chance += decisions[indexProduction].Chance;
                }
                else if (decisions[indexQuality].Chance > 0)
                {
                    decisions[indexMediate].Chance += decisions[indexQuality].Chance;
                }
                else if (decisions[indexExpand].Chance > 0)
                {
                    decisions[indexMediate].Chance += decisions[indexExpand].Chance;
                }
            }

            var savedDecisionChances = await DecisionChanceCore.UpdateAsync(decisions, true).ConfigureAwait(false);
            if (savedDecisionChances != null)
            {
                foreach (var decision in savedDecisionChances)
                {
                    await simulation.CommitLog(new SimulationLog
                    {
                        NodeId = -99,
                        Type = (int)SimulationLogType.DecisionNormalization,
                        Content = $"{decision.DecisionId} {decision.Chance} {decisionPopularity[decision.DecisionId]}"
                    }).ConfigureAwait(false);
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

        private static async Task CreateLogFile(string physPath, int id, int iteration)
        {
            var thisIterationLogs = await SimulationLogCore.GetListAsync(sl => sl.SimulationId == id && sl.IterationNumber == iteration).ConfigureAwait(false);

            if (!Directory.Exists(physPath))
            {
                Directory.CreateDirectory(physPath);
            }

            var simPath = physPath + "/Simulation " + id;

            if (!Directory.Exists(simPath))
            {
                Directory.CreateDirectory(simPath);
            }

            var logPath = simPath + "/" + iteration + ".txt";

            using (var file = new StreamWriter(logPath))
            {
                foreach (var log in thisIterationLogs)
                {
                    file.WriteLine(log);
                }
            }
        }
    }
}
