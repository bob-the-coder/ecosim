using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Models;

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
            var centroid1 = new Random().Next(0, networkSize - 1);
            var centroid2 = new Random().Next(0, centroid1 - 1);
            for (var i = 0; i < networkSize; i++)
            {
                if (i == centroid1 || i == centroid2) continue;
                links.Add(new NodeLink
                {
                    NodeId = network[centroid1].Id,
                    LinkId = network[i].Id
                });
                links.Add(new NodeLink
                {
                    NodeId = network[i].Id,
                    LinkId = network[i + 1].Id
                });
                i++;
                if (i == centroid1) i++;
                if (i == centroid2) i++;
                if (i == centroid1) i++;
                links.Add(new NodeLink
                {
                    NodeId = network[centroid2].Id,
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

        public static void ImportFromCsv(List<string> inputRows, List<NodeLink> links, List<Node> network)
        {
            return;
        }
    }
}
