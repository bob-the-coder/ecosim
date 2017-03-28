using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using DatabaseHandler.Helpers;
using Models;
using Models.Interfaces;

namespace BusinessLogic
{
    public class BaseCore
    {
        public static T Save<T>(T model, StoredProcedures procedure)
            where T : class, ISimObject, new()
        {
            var sp = new StoredProcedureBase(procedure, model);

            OperationStatus status;
            var result = StoredProcedureExecutor.GetSingleSetResult<T>(sp, out status);

            return status.Error ? null : result;
        }

        public static Node CreateNode(Node model)
        {
            var sp = new StoredProcedureBase(StoredProcedures.NodeCreate, model, ignore: new[] { "Neighbours", "ShortestPathsHeap" });

            OperationStatus status;
            var result = StoredProcedureExecutor.GetSingleSetResult<Node>(sp, out status);

            return status.Error ? null : result;
        }

        public static List<T> Create<T>(IList<T> models, StoredProcedures procedure)
            where T : class, ISimObject, new()
        {
            var sps = models.Select(model => new StoredProcedureBase(procedure, model)).ToList();
            var result = new List<T>();

            OperationStatus status;
            foreach (var sp in sps)
            {
                var mResult = StoredProcedureExecutor.GetSingleSetResult<T>(sp, out status);
                if (status.Error)
                {
                    return null;
                }
                result.Add(mResult);
            }

            return result;
        }

        public static bool StartIteration(int simId)
        {
            var sp = new StoredProcedureBase(StoredProcedures.StartIteration, new SimulationMember
            {
                SimulationId = simId
            });

            OperationStatus status;
            var result = StoredProcedureExecutor.GetSingleSetResult<SimulationMember>(sp, out status);

            return !status.Error;
        }

        public static SimulationSettings Get(SimulationMember model, StoredProcedures procedure)
        {
            var sp = new StoredProcedureBase(procedure, model);

            OperationStatus status;
            var result = StoredProcedureExecutor.GetSingleSetResult<SimulationSettings>(sp, out status);

            return status.Error ? null : result;
        }

        public static List<T> GetMemberList<T>(SimulationMember model, StoredProcedures procedure, string where = null)
            where T : class, ISimObject, new()
        {
            var sp = new StoredProcedureBase(procedure, model);

            OperationStatus status;
            var result = StoredProcedureExecutor.GetMultipleSetResult<T>(sp, out status);

            return status.Error ? null : result;
        }

        public static List<IndexSimulation> GetAllSimulations()
        {
            var sp = new StoredProcedureBase(StoredProcedures.Simulation_GetAll);

            OperationStatus status;
            var result = StoredProcedureExecutor.GetMultipleSetResult<IndexSimulation>(sp, out status);

            return (status.Error || result == null) ? new List<IndexSimulation>() : result;
        }

        public static FullSimulation GetFullSimulation(int simulationId)
        {
            var sm = new SimulationMember
            {
                SimulationId = simulationId
            };

            var simulation = Get(sm, StoredProcedures.FullSimulation_GetSimulation);
            if (simulation == null)
            {
                return null;
            }

            var result = new FullSimulation
            {
                Simulation = simulation
            };

            var nodes = GetMemberList<Node>(sm, StoredProcedures.FullSimulation_GetNodes);
            result.Network = nodes ?? new List<Node>();

            var links = GetMemberList<NodeLink>(sm, StoredProcedures.FullSimulation_GetLinks);
            result.Links = links ?? new List<NodeLink>();

            var products = GetMemberList<Product>(sm, StoredProcedures.FullSimulation_GetProducts);
            result.Products = products ?? new List<Product>();

            var productions = GetMemberList<Production>(sm, StoredProcedures.FullSimulation_GetProductions);
            result.Productions = productions ?? new List<Production>();

            var needs = GetMemberList<Need>(sm, StoredProcedures.FullSimulation_GetNeeds);
            result.Needs = needs ?? new List<Need>();

            var logs = GetMemberList<SimulationLog>(sm, StoredProcedures.FullSimulation_GetSimulationLogs);
            result.Logs = logs ?? new List<SimulationLog>();

            var decisionChances = GetMemberList<DecisionChance>(sm, StoredProcedures.FullSimulation_GetDecisionChances);
            result.DecisionChances = decisionChances ?? new List<DecisionChance>();

            return result;
        }
    }
}
