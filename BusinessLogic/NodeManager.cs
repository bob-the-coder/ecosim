using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using DatabaseHandler.Helpers;
using DatabaseHandler.Interfaces;
using DatabaseHandler.StoreProcedures;

namespace BusinessLogic
{
    public class NodeManager
    {
        public static async Task<Node> GetAsync(Guid id)
        {
            var sp = new NodeGet(id);
            OperationStatus status;
            var result = await Task.Run(() => StoredProcedureExecutor.GetSingleSetResult<Node>(sp, out status)).ConfigureAwait(false);

            return result;
        }

        public static async Task<Node> CreateAsync(Node model)
        {
            var sp = new NodeCreate(model);
            OperationStatus status;
            var result = await Task.Run(() => StoredProcedureExecutor.GetSingleSetResult<Node>(sp, out status)).ConfigureAwait(false);

            return result;
        }

        public static async Task<List<Node>> GetAllAsync()
        {
            var sp = new NodeGetAll();
            OperationStatus status;
            var result = await Task.Run(() => StoredProcedureExecutor.GetMultipleSetResult<Node>(sp, out status)).ConfigureAwait(false);

            return result;
        }

        public static async Task<List<Node>> GetListAsync(Node node)
        {
            var sp = new NodeGetList("NodeId", node.Id);
            OperationStatus status;
            var result = await Task.Run(() => StoredProcedureExecutor.GetMultipleSetResult<Node>(sp, out status)).ConfigureAwait(false);

            return result;
        }

        public static async Task<VisJsGraph> GetGraphAsync()
        {
            OperationStatus status;
            var nodes = await Task.Run(
                        () => StoredProcedureExecutor.GetMultipleSetResult<VisJsNode>(
                            new NodeGetAllVisJs(), out status)).ConfigureAwait(false);
            var edges = await Task.Run(
                        () => StoredProcedureExecutor.GetMultipleSetResult<VisJsEdge>(
                            new LinkGetAllVisJs(), out status)).ConfigureAwait(false);

            return new VisJsGraph
            {
                Nodes = nodes,
                Edges = edges
            };
        }

        public static async Task AddLinks(Node n, List<Guid> linkIds)
        {
            await Task.Run(() =>
            {
                var sps = new List<StoredProcedureBase>();
                linkIds.ForEach(l =>
                {
                    sps.Add(new NodeLinkCreate(n.Id, l));
                });
                StoredProcedureExecutor.ExecuteNoQueryAsTransaction(sps);
            }).ConfigureAwait(false);
        }

        public static async Task NetworkInitialCreate(List<Node> network, List<NodeLink> links)
        {
            await Task.Run(() =>
            {
                var sps = new List<StoredProcedureBase>();
                network.ForEach(node =>
                {
                    sps.Add(new NodeCreate(node));
                });
                links.ForEach(link => {sps.Add(new NodeLinkCreate(link.NodeId, link.LinkId));});

                StoredProcedureExecutor.ExecuteNoQueryAsTransaction(sps);
            }).ConfigureAwait(false);
        }
    }
}
