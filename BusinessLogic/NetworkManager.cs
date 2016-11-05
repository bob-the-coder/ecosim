using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BusinessLogic.Models;

namespace BusinessLogic
{
    public class NetworkManager
    {
        public static async Task<Dictionary<Guid, Guid>> GetBfsList(Node start)
        {
            var network = await NodeManager.GetAllAsync().ConfigureAwait(false);
            var networkSize = network.Count;
            var bfsResult = new Dictionary<Guid, Guid> { { start.Id, Guid.Empty} };

            bfsResult = await AddNodesToBfsList(bfsResult, networkSize, new List<Node> { start }).ConfigureAwait(false);

            return bfsResult;
        }

        public static async Task<string> GetBfsPath(Node dest, IDictionary<Guid, Guid> bfsResult)
        {
            if (dest == null)
            {
                return null;
            }

            var res = $"{dest.Name}";

            if (bfsResult == null || bfsResult.Count == 0)
            {
                return res;
            }

            while (true)
            {
                if (!bfsResult.ContainsKey(dest.Id))
                {
                    return res;
                }
                dest = await NodeManager.GetAsync(bfsResult[dest.Id]).ConfigureAwait(false);
                if (dest == null)
                {
                    return res;
                }
                res = $"{dest.Name} -> {res}";
            }
        }

        private static async Task<Dictionary<Guid, Guid>> AddNodesToBfsList(Dictionary<Guid, Guid> bfsList, int networkSize, List<Node> startNodes)
        {
            if (bfsList.Count == networkSize || startNodes == null || startNodes.Count == 0)
            {
                return bfsList;
            }
            var nextIterationNodes = new List<Node>();

            foreach (var startNode in startNodes)
            {
                var neighbours = await NodeManager.GetListAsync(startNode).ConfigureAwait(false);
                if(neighbours == null)
                {
                    continue;
                }
                foreach (var neighbour in neighbours)
                {
                    if (bfsList.ContainsKey(neighbour.Id))
                    {
                        continue;
                    }
                    bfsList.Add(neighbour.Id, startNode.Id);
                    nextIterationNodes.Add(neighbour);
                }
            }

            if (nextIterationNodes.Count == 0)
            {
                return bfsList;
            }

            return await AddNodesToBfsList(bfsList, networkSize, nextIterationNodes).ConfigureAwait(false);
        }
    }
}
