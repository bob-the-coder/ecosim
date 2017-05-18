using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models;
using Models;

namespace BusinessLogic
{
    public static class NetworkManager
    {
        public static void GetShortestPathsHeap(this Node start, List<Node> network)
        {
            var networkSize = network.Count;
            var bfsResult = new Dictionary<int, int> { { start.Id, -1 } };

            bfsResult = GetShortestPathsByNeighbours(bfsResult, networkSize, new List<Node> { start }, network);

            start.ShortestPathsHeap = bfsResult;
        }


        public static List<Node> GetShortestPathToNode(this Node origin, Node dest, List<Node> network)
        {
            if (origin == null || dest == null)
            {
                return null;
            }

            if (origin.ShortestPathsHeap == null || origin.ShortestPathsHeap.Count == 0)
            {
                return null;
            }

            if (!origin.ShortestPathsHeap.ContainsKey(dest.Id))
            {
                return null;
            }

            var res = new List<Node>();

            while (true)
            {
                if (!origin.ShortestPathsHeap.ContainsKey(dest.Id))
                {
                    return null;
                }
                if (origin.ShortestPathsHeap[dest.Id] == -1)
                {
                    res.Add(origin);
                    return res;
                }
                res.Add(dest);
                dest = network.First(node => node.Id == origin.ShortestPathsHeap[dest.Id]);
            }
        }

        private static Dictionary<int, int> GetShortestPathsByNeighbours(Dictionary<int, int> heap, int networkSize, List<Node> startNodes, List<Node> network)
        {
            while (true)
            {
                if (heap.Count == networkSize || startNodes == null || startNodes.Count == 0)
                {
                    return heap;
                }
                var nextIterationNodes = new List<Node>();

                foreach (var startNode in startNodes)
                {
                    var neighbours = startNode.Neighbours;
                    if (neighbours == null)
                    {
                        continue;
                    }
                    foreach (var neighbour in neighbours)
                    {
                        if (heap.ContainsKey(neighbour.Id))
                        {
                            continue;
                        }
                        heap.Add(neighbour.Id, startNode.Id);
                        nextIterationNodes.Add(network.First(node => node.Id == neighbour.Id));
                    }
                }

                if (nextIterationNodes.Count == 0)
                {
                    return heap;
                }

                startNodes = nextIterationNodes;
            }
        }
    }
}
