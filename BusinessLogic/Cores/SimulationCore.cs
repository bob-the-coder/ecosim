using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.ModelCore;
using BusinessLogic.Models;
using BusinessLogic.TypeManagement;
using DataLayer.Implementation;

namespace BusinessLogic.Cores
{
    public class SimulationCore : BaseSinglePkCore<SimulationRepository, Simulation, DataLayer.Simulation>
    {
        public static async Task<FullSimulation> GetFullSimulationAsync(int id)
        {
            var res = new FullSimulation();
            var sim = await GetAsync(id).ConfigureAwait(false);

            res.Simulation = sim.CopyTo<SimulationSettings>();

            var existingLogs = await SimulationLogCore.GetListAsync(sl => sl.SimulationId == id).ConfigureAwait(false);
            res.Simulation.LatestIteration = (existingLogs != null && existingLogs.Any())
                ? existingLogs.Max(sl => sl.IterationNumber)
                : 0;
            res.Network = (await NodeCore.GetListAsync(n => n.SimulationId == id).ConfigureAwait(false)).ToList();
            res.Products = (await ProductCore.GetAllAsync().ConfigureAwait(false)).ToList();

            var allProductions = await ProductionCore.GetAllAsync().ConfigureAwait(false);
            var allNeeds = await NeedCore.GetAllAsync().ConfigureAwait(false);

            res.Productions = allProductions.Where(pr => res.Network.Exists(n => n.Id == pr.NodeId)).ToList();
            res.Needs = allNeeds.Where(pr => res.Network.Any(n => n.Id == pr.NodeId)).ToList();

            res.Links = new List<NodeLink>(await NodeLinkCore.GetListAsync(nl => nl.SimulationId == sim.Id).ConfigureAwait(false));
            res.DecisionChances = (await DecisionChanceCore.GetListAsync(dc => dc.SimulationId == id).ConfigureAwait(false)).ToList();

            res.Logs = new List<SimulationLog>();

            return res;
        }

        public static async Task<SimulationSettings> CreateAsync(SimulationSettings model, bool value = false)
        {
            var result = await CreateAsync(model.CopyTo<Simulation>(), value).ConfigureAwait(false);
            return result.CopyTo<SimulationSettings>();
        }

        public static async Task<List<IndexSimulation>> GetIndexAsync()
        {
            var result = new List<IndexSimulation>();
            var simulations = await GetAllAsync().ConfigureAwait(false);

            foreach (var simulation in simulations)
            {
                var fullSim = await GetFullSimulationAsync(simulation.Id).ConfigureAwait(false);

                result.Add(new IndexSimulation
                {
                    Id = fullSim.Simulation.Id,
                    IterationsDone = fullSim.Simulation.LatestIteration,
                    Name = fullSim.Simulation.Name,
                    NeedCount = fullSim.Needs.Count,
                    LinkCount = fullSim.Links.Count,
                    NodeCount = fullSim.Network.Count,
                    ProductCount = fullSim.Products.Count,
                    ProductionCount = fullSim.Productions.Count
                });
            }

            return result;
        }
    }
}
