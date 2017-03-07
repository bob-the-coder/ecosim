using System;
using System.Collections.Generic;
using Models;
using DatabaseHandler.Helpers;
using DatabaseHandler.StoreProcedures;
using Models.Interfaces;

namespace BusinessLogic
{
    public class NodeManager
    {
        public static Node Get(int id)
        {
            var sp = new NodeGet(id);
            OperationStatus status;
            var result = StoredProcedureExecutor.GetSingleSetResult<Node>(sp, out status);

            return result;
        }

        public static Node Create(Node model)
        {
            var sp = new NodeCreate(model);
            OperationStatus status;
            var result = StoredProcedureExecutor.GetSingleSetResult<Node>(sp, out status);

            return result;
        }

        public static List<Node> GetAll()
        {
            var sp = new NodeGetAll();
            OperationStatus status;
            var result = StoredProcedureExecutor.GetMultipleSetResult<Node>(sp, out status);

            return result;
        }

        public static List<Node> GetList(Node node)
        {
            var sp = new NodeGetList("NodeId", node.Id);
            OperationStatus status;
            var result = StoredProcedureExecutor.GetMultipleSetResult<Node>(sp, out status);

            return result;
        }

        public static VisJsGraph GetGraph()
        {
            OperationStatus status;
            var nodes = StoredProcedureExecutor.GetMultipleSetResult<VisJsNode>(new NodeGetAllVisJs(), out status);
            var edges = StoredProcedureExecutor.GetMultipleSetResult<VisJsEdge>(
                new LinkGetAllVisJs(), out status);

            return new VisJsGraph
            {
                Nodes = nodes,
                Edges = edges
            };
        }

        public static void AddLinks(Node n, List<int> linkIds)
        {
            var sps = new List<StoredProcedureBase>();
            linkIds.ForEach(l =>
            {
                sps.Add(new NodeLinkCreate(n.Id, l));
            });
            StoredProcedureExecutor.ExecuteNoQueryAsTransaction(sps);
        }

        public static List<Node> AppendToNetwork(List<Node> network = null, List<NodeLink> links = null)
        {
            if (network != null || links != null)
            {
                var sps = new List<StoredProcedureBase>();
                network?.ForEach(node =>
                {
                    sps.Add(new NodeCreate(node));
                });

                links?.ForEach(link => { sps.Add(new NodeLinkCreate(link.NodeId, link.LinkId)); });

                if (!StoredProcedureExecutor.ExecuteNoQueryAsTransaction(sps))
                {
                    return null;
                }
            }

            OperationStatus os;
            network = StoredProcedureExecutor.GetMultipleSetResult<Node>(new NodeGetAll(), out os);

            return os.Error ? null : network;
        }

        public static List<Product> GetAllProducts()
        {
            OperationStatus or;
            var result = StoredProcedureExecutor.GetMultipleSetResult<Product>(new ProductGetAll(), out or);

            return or.Error ? null : result;
        }

        public static List<Need> GetAllNeeds()
        {
            OperationStatus or;
            var result = StoredProcedureExecutor.GetMultipleSetResult<Need>(new NeedGetAll(), out or);

            return or.Error ? null : result;
        }

        public static List<Production> GetAllProductions()
        {
            OperationStatus or;
            var result = StoredProcedureExecutor.GetMultipleSetResult<Production>(new ProductionGetAll(), out or);

            return or.Error ? null : result;
        }

        public static List<NodeLink> GetAllLinks()
        {
            OperationStatus or;
            var result = StoredProcedureExecutor.GetMultipleSetResult<NodeLink>(new NodeLinkGetAll(), out or);

            return or.Error ? null : result;
        }
    }
}
