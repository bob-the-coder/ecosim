using System.Collections.Generic;
using AutoMapper;
using BusinessLogic.Interfaces;
using DataLayer.Interfaces;

namespace BusinessLogic.TypeManagement
{
    public static class DataAdapterService
    {
        private static IMapper mMapper;
        private static bool mIsConfigured;

        static DataAdapterService()
        {
            InitializeMapper();
        }

        private static void InitializeMapper()
        {
            if (mIsConfigured)
            {
                return;
            }

            var mapperConfiguration = new MapperConfiguration(config =>
            {
                DasConfigurator.ConfigureNode(config);
                DasConfigurator.ConfigureNodeLink(config);
                DasConfigurator.ConfigureProduct(config);
                DasConfigurator.ConfigureProduction(config);
                DasConfigurator.ConfigureNeed(config);
                DasConfigurator.ConfigureDecision(config);
                DasConfigurator.ConfigureDecisionChance(config);
                DasConfigurator.ConfigureSimulation(config);
                DasConfigurator.ConfigureSimulationLog(config);

                config.ForAllMaps((mapType, mapperExpression) => { mapperExpression.MaxDepth(2); });
            });

            mapperConfiguration.AssertConfigurationIsValid();
            mMapper = mapperConfiguration.CreateMapper();

            mIsConfigured = true;
        }

        #region IModel

        /// <summary>
        ///     Copies the object to a new entity of type <typeparamref name="TDestType" /> by mapping their properties one-to-one
        /// </summary>
        /// <typeparam name="TDestType">The type of returned entity</typeparam>
        /// <param name="entity">The entity that will be copied to the new type</param>
        /// <returns>An entity of type <typeparamref name="TDestType" /> that contains all the properties from the source object</returns>
        public static TDestType CopyTo<TDestType>(this IModel entity) where TDestType : class
        {
            return entity != null ? mMapper.Map<TDestType>(entity) : null;
        }

        /// <summary>
        ///     Copies the object to an existing entity of type <typeparamref name="TDestType" /> by mapping their properties one-to-one
        /// </summary>
        /// <typeparam name="TDestType">The type of destination entity</typeparam>
        /// <param name="entity">The entity that will be copied</param>
        /// <param name="destination">The entity that will be modified</param>
        public static void CopyTo<TDestType>(this IModel entity, TDestType destination) where TDestType : class
        {
            if (entity == null)
            {
                return;
            }

            mMapper.Map(entity, destination);
        }

        /// <summary>
        ///     Copies the list of objects to a new list with entities of type <typeparamref name="TDestType" /> by mapping their
        ///     properties one-to-one,
        ///     in the same order as found in the source list
        /// </summary>
        /// <typeparam name="TDestType">The type of the entities in the returned list</typeparam>
        /// <param name="entityList">The list of entities that will be copied to the new type</param>
        /// <returns>
        ///     A list with entities of type <typeparamref name="TDestType" /> that contain all the properties from the source
        ///     objects
        /// </returns>
        public static IList<TDestType> CopyTo<TDestType>(this IEnumerable<IModel> entityList) where TDestType : class
        {
            return entityList != null ? mMapper.Map<IList<TDestType>>(entityList) : null;
        }

        #endregion

        #region IDataAccessObject

        /// <summary>
        ///     Copies the object to a new entity of type <typeparamref name="TDestType" /> by mapping their properties one-to-one
        /// </summary>
        /// <typeparam name="TDestType">The type of returned entity</typeparam>
        /// <param name="entity">The entity that will be copied to the new type</param>
        /// <returns>An entity of type <typeparamref name="TDestType" /> that contains all the properties from the source object</returns>
        public static TDestType CopyTo<TDestType>(this IDataAccessObject entity) where TDestType : class
        {
            return entity != null ? mMapper.Map<TDestType>(entity) : null;
        }

        /// <summary>
        ///     Copies the object to an existing entity of type <typeparamref name="TDestType" /> by mapping their properties one-to-one
        /// </summary>
        /// <typeparam name="TDestType">The type of destination entity</typeparam>
        /// <param name="entity">The entity that will be copied</param>
        /// <param name="destination">The entity that will be modified</param>
        public static void CopyTo<TDestType>(this IDataAccessObject entity, TDestType destination) where TDestType : class
        {
            if (entity == null)
            {
                return;
            }

            mMapper.Map(entity, destination);
        }

        /// <summary>
        ///     Copies the list of objects to a new list with entities of type <typeparamref name="TDestType" /> by mapping their
        ///     properties one-to-one,
        ///     in the same order as found in the source list
        /// </summary>
        /// <typeparam name="TDestType">The type of the entities in the returned list</typeparam>
        /// <param name="entityList">The list of entities that will be copied to the new type</param>
        /// <returns>
        ///     A list with entities of type <typeparamref name="TDestType" /> that contain all the properties from the source
        ///     objects
        /// </returns>
        public static IList<TDestType> CopyTo<TDestType>(this IEnumerable<IDataAccessObject> entityList) where TDestType : class
        {
            return entityList != null ? mMapper.Map<IList<TDestType>>(entityList) : null;
        }

        #endregion
    }
}
