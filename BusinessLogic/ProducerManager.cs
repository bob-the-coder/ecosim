using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace BusinessLogic
{
    public class ProducerManager
    {
        private static readonly Random Rng = new Random();

        public static List<Node> CreateByAmount(List<Node> network, int producerSelectionBias = 0)
        {
            var producersCount = Math.Min(producerSelectionBias, network.Count);
            var producers = network.GetRange(0, producersCount - 1);

            return producers;
        }

        public static List<Node> CreateByNetworkPercentage(List<Node> network, int producerSelectionBias = 0)
        {
            var percentage = Math.Min(Math.Abs(producerSelectionBias), 100);
            var producersCount = percentage * network.Count / 100;
            var producers = new List<Node>();

            while (producersCount > 0)
            {
                var nodeIndex = Rng.Next(0, network.Count - 1);

                if (producers.Any(node => node.Id == network[nodeIndex].Id))
                {
                    continue;
                }

                producers.Add(network[nodeIndex]);
                producersCount--;
            }

            return producers;
        }

        public static List<Node> CreateByChance(List<Node> network, int producerSelectionBias = 0)
        {
            var chance = producerSelectionBias % 100;
            var producers = new List<Node>();

            foreach (var node in network)
            {
                var prob = Rng.Next(0, 100);
                if (prob <= chance)
                {
                    producers.Add(node);
                }
            }

            return producers;
        }
    }
}
