using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Models;

namespace BusinessLogic
{
    public class NetworkCreationPatterns
    {
        private static readonly Random Rng = new Random();

        public static void UsePatternCircular(int networkSize, List<NodeLink> links, List<Node> network)
        {
            for (var i = 0; i < networkSize - 1; i++)
            {
                links.Add(new NodeLink
                {
                    NodeId = network[i].Id,
                    LinkId = network[i + 1].Id
                });
            }
            links.Add(new NodeLink
            {
                NodeId = network[networkSize - 1].Id,
                LinkId = network[0].Id
            });
        }

        public static void UsePatternCentroid(int networkSize, List<NodeLink> links, List<Node> network)
        {
            var centroid = new Random().Next(0, networkSize - 1);
            for (var i = 0; i < networkSize; i++)
            {
                if (i == centroid)
                {
                    continue;
                }

                links.Add(new NodeLink
                {
                    NodeId = network[centroid].Id,
                    LinkId = network[i].Id
                });
            }
        }

        public static void UsePatternRandom(int networkSize, List<NodeLink> links, List<Node> network)
        {
            const int prob = 98;
            for (var i = 0; i < networkSize; i++)
            {
                for (var j = 0; j < networkSize; j++)
                {
                    if (i == j || links.Any(
                        link => (link.NodeId == network[i].Id && link.LinkId == network[j].Id)
                                || (link.NodeId == network[j].Id && link.LinkId == network[i].Id)))
                    {
                        continue;
                    }
                    var p = Rng.Next(1, 100);
                    if (p >= prob)
                    {
                        links.Add(new NodeLink
                        {
                            NodeId = network[i].Id,
                            LinkId = network[j].Id
                        });
                    }
                }
            }
        }

        public static void UsePatternSmallWorld(int networkSize, List<NodeLink> links, List<Node> network, int k, int p)
        {
            if (network == null || network.Count == 0)
            {
                return;
            }

            for (var i = 0; i < networkSize; i++)
            {
                links.Add(new NodeLink
                {
                    NodeId = network[i].Id,
                    LinkId = network[(i + 1) % networkSize].Id
                });

                for (var j = 1; j <= k; j++)
                {
                    var nextKIndex = (i + j + 1) % networkSize;
                    var prevKIndex = (i - j - 1) < 0 ? (networkSize + i - j - 1) % networkSize : (i - j - 1);

                    links.Add(new NodeLink
                    {
                        NodeId = network[i].Id,
                        LinkId = network[nextKIndex].Id
                    });

                    links.Add(new NodeLink
                    {
                        NodeId = network[i].Id,
                        LinkId = network[prevKIndex].Id
                    });
                }
            }

            if (p <= 0)
            {
                return;
            }

            for (var i = 0; i < networkSize; i++)
            {
                var iLinks = links.Where(link => link.NodeId == network[i].Id).ToList();

                foreach (var link in iLinks)
                {
                    var prob = Rng.Next(0, 100);
                    if (prob > p)
                    {
                        continue;
                    }

                    var nonNeighbours = links.Where(l => l.NodeId != network[i].Id && l.LinkId != network[i].Id).ToList();

                    var newLinkIndex = Rng.Next(0, nonNeighbours.Count - 1);

                    links.Remove(link);

                    links.Add(new NodeLink
                    {
                        NodeId = network[i].Id,
                        LinkId = nonNeighbours[newLinkIndex].NodeId
                    });

                    break;
                }
            }
        }

        public static void UsePatternGrid(List<NodeLink> links, List<Node> network, int n, int m)
        {
            if (n == 0 || m == 0 || network?.Count != n * m)
            {
                return;
            }

            var nodeLines = new List<List<Node>>();
            for (var i = 0; i < n; i++)
            {
                var line = new List<Node>();
                for (var j = 0; j < m; j++)
                {
                    line.Add(network[i * m + j]);
                }
                nodeLines.Add(line);
            }

            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < m - 1; j++)
                {
                    links.Add(new NodeLink
                    {
                        NodeId = nodeLines[i][j].Id,
                        LinkId = nodeLines[i][j + 1].Id
                    });
                    links.Add(new NodeLink
                    {
                        NodeId = nodeLines[i][j + 1].Id,
                        LinkId = nodeLines[i][j].Id
                    });
                }

                if (i == n - 1)
                {
                    break;
                }

                for (var j = 0; j < m; j++)
                {
                    links.Add(new NodeLink
                    {
                        NodeId = nodeLines[i][j].Id,
                        LinkId = nodeLines[i + 1][j].Id
                    });
                    links.Add(new NodeLink
                    {
                        NodeId = nodeLines[i + 1][j].Id,
                        LinkId = nodeLines[i][j].Id
                    });
                }
            }
        }

        public static void ImportFromCsv(List<string> inputRows, List<NodeLink> links, List<Node> network)
        {
            return;
        }
    }
}
