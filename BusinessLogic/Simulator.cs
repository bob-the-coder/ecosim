using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Configuration;
using BusinessLogic.Economics;
using Models;
using DatabaseHandler.StoreProcedures;
using DatabaseHandler.Helpers;

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

        public static bool CommitChanges(List<Node> network, List<Production> productions = null, List<Need> needs = null)
        {
            var procedures = new List<StoredProcedureBase>();
            network.ForEach(node => procedures.Add(new NodeUpdate(node)));
            productions?.ForEach(production => procedures.Add(new ProductionUpdate(production)));
            needs?.ForEach(need => procedures.Add(new NeedUpdate(need)));

            return StoredProcedureExecutor.ExecuteNoQueryAsTransaction(procedures);
        }

        public static void SimulateIteration(Guid id, List<string> log = null)
        {
            var network = NodeManager.GetAll();
            var links = NodeManager.GetAllLinks();
            var simSettings = SimulationManager.GetSettings(id);

            LinkNodes(network, links);

            var allProductions = NodeManager.GetAllProductions();
            var needs = NodeManager.GetAllNeeds();

            network.ForEach(node => log?.AddRange(node.LogInfo()));

            log?.Add("Transaction Phase");
            log?.Add("");

            foreach (var node in network)
            {
                var nodeNeeds = needs.Where(need => need.NodeId == node.Id).ToList();
                if (nodeNeeds.Count == 0)
                {
                    continue;
                }

                node.GetShortestPathsHeap(network);

                foreach (var need in nodeNeeds)
                {
                    var chanceToFulfill = Rng.Next(1, 100);

                    if (chanceToFulfill <= need.Priority)
                    {
                        FulfillNeed(node, need, network, node.ShortestPathsHeap, allProductions, simSettings, log);
                    }
                }
            }

            var result = CommitChanges(network, allProductions, needs);
            log?.Add($"{result}");
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

        private static void FulfillNeed(Node node,
            Need need,
            List<Node> network,
            Dictionary<int, int> nodeBfsResult,
            List<Production> allProductions,
            Simulation simSettings,
            List<string> log)
        {
            var selectedProducer = node.GetClosestProducer(network, allProductions, need);

            if (selectedProducer == null)
            {
                return;
            }

            var selectedProduction =
                allProductions.First(p => p.NodeId == selectedProducer.Id && p.ProductId == need.ProductId);

            var pathToBuyer = node.GetShortestPathToNode(selectedProducer, network);

            node.BuysFrom(need, selectedProducer, selectedProduction, pathToBuyer.GetRange(1, pathToBuyer.Count - 1), simSettings, log);
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
