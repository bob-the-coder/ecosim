using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using BusinessLogic.TypeManagement;
using BusinessLogic.Workflow;
using DataLayer.Interfaces;
using DataLayer.Repositories;

namespace BusinessLogic.ModelCore
{
    public abstract class BaseSinglePkCore<TRepo, TModel, TEntity> : BaseCore<TRepo, TModel, TEntity>
        where TRepo : BaseSinglePkRepository<TEntity>
        where TEntity : class, ISinglePkDataAccessObject, new()
        where TModel : class, ISinglePkModel, new()
    {
        public static async Task<TModel> GetAsync(int id, IList<string> navigationProperties = null)
        {
            using (var repository = RepoUnitOfWork.CreateRepository<TRepo>())
            {
                var entities = await repository.GetAsync(id, navigationProperties).ConfigureAwait(false);

                return entities.CopyTo<TModel>();
            }
        }

        public static async Task DeleteAsync(int id)
        {
            using (var repository = RepoUnitOfWork.CreateTrackingRepository<TRepo>())
            {
                await repository.DeleteAsync(id).ConfigureAwait(false);
            }
        }
    }
}
