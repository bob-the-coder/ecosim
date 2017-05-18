using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using BusinessLogic.TypeManagement;
using BusinessLogic.Workflow;
using BusinessLogic.Workflow.Interfaces;
using DataLayer.Interfaces;
using DataLayer.Repositories;

namespace BusinessLogic.ModelCore
{
    public abstract class BaseCore<TRepo, TModel, TEntity>
        where TRepo : BaseRepository<TEntity>
        where TEntity : class, IDataAccessObject, new()
        where TModel : class, IModel, new()
    {
        #region Singular structure result methods

        public static async Task<int> CountAsync()
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.CountAsync().ConfigureAwait(false);
            }
        }

        public static async Task<int> CountAsync(Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.CountAsync(@where).ConfigureAwait(false);
            }
        }

        #region Sum

        public async Task<int> SumAsync(Expression<Func<TEntity, int>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<int?> SumAsync(Expression<Func<TEntity, int?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<int> SumAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<int?> SumAsync(Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<float> SumAsync(Expression<Func<TEntity, float>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<float?> SumAsync(Expression<Func<TEntity, float?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<float> SumAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<float?> SumAsync(Expression<Func<TEntity, float?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<double> SumAsync(Expression<Func<TEntity, double>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<double?> SumAsync(Expression<Func<TEntity, double?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<double> SumAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<double?> SumAsync(Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.SumAsync(selector, @where).ConfigureAwait(false);
            }
        }

        #endregion

        #region Min

        public async Task<int> MinAsync(Expression<Func<TEntity, int>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<int?> MinAsync(Expression<Func<TEntity, int?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<int> MinAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<int?> MinAsync(Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<float> MinAsync(Expression<Func<TEntity, float>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<float?> MinAsync(Expression<Func<TEntity, float?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<float> MinAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<float?> MinAsync(Expression<Func<TEntity, float?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<double> MinAsync(Expression<Func<TEntity, double>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<double?> MinAsync(Expression<Func<TEntity, double?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<double> MinAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<double?> MinAsync(Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<decimal> MinAsync(Expression<Func<TEntity, decimal>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<decimal?> MinAsync(Expression<Func<TEntity, decimal?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<decimal> MinAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<decimal?> MinAsync(Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MinAsync(selector, @where).ConfigureAwait(false);
            }
        }

        #endregion

        #region Max

        public async Task<int> MaxAsync(Expression<Func<TEntity, int>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<int?> MaxAsync(Expression<Func<TEntity, int?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<int> MaxAsync(Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<int?> MaxAsync(Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<float> MaxAsync(Expression<Func<TEntity, float>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<float?> MaxAsync(Expression<Func<TEntity, float?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<float> MaxAsync(Expression<Func<TEntity, float>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<float?> MaxAsync(Expression<Func<TEntity, float?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<double> MaxAsync(Expression<Func<TEntity, double>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<double?> MaxAsync(Expression<Func<TEntity, double?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<double> MaxAsync(Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<double?> MaxAsync(Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<decimal> MaxAsync(Expression<Func<TEntity, decimal>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<decimal?> MaxAsync(Expression<Func<TEntity, decimal?>> selector)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector).ConfigureAwait(false);
            }
        }

        public async Task<decimal> MaxAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector, @where).ConfigureAwait(false);
            }
        }

        public async Task<decimal?> MaxAsync(Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> where)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                return await repository.MaxAsync(selector, @where).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region CRUD

        public static async Task<IList<TModel>> GetAllAsync(IList<string> navigationProperties = null)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                var entities = await repository.GetAllAsync(navigationProperties).ConfigureAwait(false);

                return entities.CopyTo<TModel>();
            }
        }

        public static IConfigurableQuery<TEntity, TModel> GetAllQuery(IList<string> navigationProperties = null)
        {
            var repository = RepoUnitOfWork.CreateRepository<TRepo>();
            var query = repository.GetAllQuery(navigationProperties);

            return new ConfigurableQuery<TEntity, TModel>(repository.Context, query);
        }

        public static async Task<IList<TModel>> GetListAsync(Expression<Func<TEntity, bool>> where, IList<string> navigationProperties = null)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                var entities = await repository.GetListAsync(@where, navigationProperties).ConfigureAwait(false);

                return entities.CopyTo<TModel>();
            }
        }

        public static IConfigurableQuery<TEntity, TModel> GetListQuery(Expression<Func<TEntity, bool>> where, IList<string> navigationProperties = null)
        {
            var repository = RepoUnitOfWork.CreateRepository<TRepo>();
            var query = repository.GetListQuery(@where, navigationProperties);

            return new ConfigurableQuery<TEntity, TModel>(repository.Context, query);
        }

        public static async Task<TModel> CreateAsync(TModel model, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            using (var repository = RepoUnitOfWork.CreateTrackingRepository<TRepo>())
            {
                var entity = model.CopyTo<TEntity>();

                entity = await repository.CreateAsync(entity, refreshFromDb, navigationProperties).ConfigureAwait(false);

                return entity.CopyTo<TModel>();
            }
        }

        public static async Task<IList<TModel>> CreateAsync(IList<TModel> modelCollection, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            using (var repository = RepoUnitOfWork.CreateTrackingRepository<TRepo>())
            {
                var entityCollection = modelCollection.CopyTo<TEntity>();

                entityCollection = await repository.CreateAsync(entityCollection, refreshFromDb, navigationProperties).ConfigureAwait(false);

                return entityCollection.CopyTo<TModel>();
            }
        }

        public static async Task<TModel> UpdateAsync(TModel model, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            using (var repository = RepoUnitOfWork.CreateTrackingRepository<TRepo>())
            {
                var entity = model.CopyTo<TEntity>();

                entity = await repository.UpdateAsync(entity, refreshFromDb, navigationProperties).ConfigureAwait(false);

                return entity.CopyTo<TModel>();
            }
        }

        public static async Task<IList<TModel>> UpdateAsync(IList<TModel> modelCollection, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            using (var repository = RepoUnitOfWork.CreateTrackingRepository<TRepo>())
            {
                var entityCollection = modelCollection.CopyTo<TEntity>();

                entityCollection = await repository.UpdateAsync(entityCollection, refreshFromDb, navigationProperties).ConfigureAwait(false);

                return entityCollection.CopyTo<TModel>();
            }
        }

        public static async Task<bool> DeleteAsync(TModel model)
        {
            using (var repository = RepoUnitOfWork.CreateTrackingRepository<TRepo>())
            {
                var entity = model.CopyTo<TEntity>();

                return await repository.DeleteAsync(entity).ConfigureAwait(false);
            }
        }

        public static async Task<bool> DeleteAsync(IList<TModel> modelCollection)
        {
            using (var repository = RepoUnitOfWork.CreateTrackingRepository<TRepo>())
            {
                var entityCollection = modelCollection.CopyTo<TEntity>();

                return await repository.DeleteAsync(entityCollection).ConfigureAwait(false);
            }
        }

        public static async Task<Response> DeleteAllAsync()
        {
            using (var repository = RepoUnitOfWork.CreateTrackingRepository<TRepo>())
            {
                var entities = await repository.GetAllAsync().ConfigureAwait(false);

                await repository.DeleteAsync(entities).ConfigureAwait(false);

                entities = await repository.GetAllAsync().ConfigureAwait(false);
                if (entities != null && entities.Any())
                {
                    return ResponseFactory.CreateResponse(false, ResponseCode.ErrorAnErrorOccurred);
                }

                return ResponseFactory.CreateResponse(true, ResponseCode.Success);
            }
        }

        #endregion
    }
}
