using System;
using System.Collections.Generic;
using AutoMapper;
using BusinessLogic.Models;

namespace BusinessLogic.TypeManagement
{
    public static class DasConfigurator
    {
        internal static void ConfigureNode(IMapperConfigurationExpression config)
        {
            config.CreateMap<DataLayer.Node, Node>()
                .ForMember(m => m.ShortestPathsHeap, opt => opt.Ignore())
                .ForMember(m => m.Neighbours, opt => opt.Ignore());
            config.CreateMap<Node, DataLayer.Node>();
        }

        internal static void ConfigureNodeLink(IMapperConfigurationExpression config)
        {
            config.CreateMap<DataLayer.NodeLink, NodeLink>();
            config.CreateMap<NodeLink, DataLayer.NodeLink>();
        }

        internal static void ConfigureProduct(IMapperConfigurationExpression config)
        {
            config.CreateMap<DataLayer.Product, Product>();
            config.CreateMap<Product, DataLayer.Product>();
        }

        internal static void ConfigureProduction(IMapperConfigurationExpression config)
        {
            config.CreateMap<DataLayer.Production, Production>();
            config.CreateMap<Production, DataLayer.Production>();
        }

        internal static void ConfigureNeed(IMapperConfigurationExpression config)
        {
            config.CreateMap<DataLayer.Need, Need>();
            config.CreateMap<Need, DataLayer.Need>();
        }

        internal static void ConfigureDecision(IMapperConfigurationExpression config)
        {
            config.CreateMap<DataLayer.Decision, Decision>();
            config.CreateMap<Decision, DataLayer.Decision>();
        }

        internal static void ConfigureDecisionChance(IMapperConfigurationExpression config)
        {
            config.CreateMap<DataLayer.DecisionChance, DecisionChance>();
            config.CreateMap<DecisionChance, DataLayer.DecisionChance>();
        }

        internal static void ConfigureSimulation(IMapperConfigurationExpression config)
        {
            config.CreateMap<DataLayer.Simulation, Simulation>();
            config.CreateMap<Simulation, DataLayer.Simulation>();

            config.CreateMap<SimulationSettings, Simulation>();
            config.CreateMap<Simulation, SimulationSettings>()
                .ForMember(m => m.DecisionLookBack, opt => opt.MapFrom(o => 1))
                .ForMember(m => m.LatestIteration, opt => opt.MapFrom(o => 0));
        }

        internal static void ConfigureSimulationLog(IMapperConfigurationExpression config)
        {
            config.CreateMap<DataLayer.SimulationLog, SimulationLog>();
            config.CreateMap<SimulationLog, DataLayer.SimulationLog>();
        }

        #region Item configuration extension methods

        private static void Configure<T>(this IEnumerable<T> items, Action<T> applyConfiguration)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                applyConfiguration(item);
            }
        }

        private static void Configure<T>(this T item, Action<T> applyConfiguration)
        {
            if (item == null)
            {
                return;
            }

            applyConfiguration(item);
        }

        #endregion
    }
}
