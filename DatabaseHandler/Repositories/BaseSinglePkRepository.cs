using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseHandler.Interfaces;
using Logging;

namespace DatabaseHandler.Repositories
{
    public abstract class BaseSinglePkRepository<T> : BaseRepository<T>
        where T : class, ISinglePkDataAccessObject, new()
    {
        public async Task<T> GetAsync(int id, IList<string> navigationProperties = null)
        {
            return await GetSingleAsync(entity => entity.Id == id, navigationProperties).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id).ConfigureAwait(false);

            if (entity == null)
            {
                return;
            }

            await RemoveAsync(entity).ConfigureAwait(false);
        }

        #region Overrides

        public override Task<T> CreateAsync(T entity, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            if (entity.Id < 0)
            {
                entity.Id = 0;
            }

            return base.CreateAsync(entity, refreshFromDb, navigationProperties);
        }

        public override Task<IList<T>> CreateAsync(IList<T> entities, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            Parallel.ForEach(entities.Where(entity => entity.Id < 0), entity => { entity.Id = 0; });

            return base.CreateAsync(entities, refreshFromDb, navigationProperties);
        }

        protected override async Task<T> FetchFromDbAsync(T entity, IList<string> navigationProperties = null)
        {
            return await GetAsync(entity.Id, navigationProperties).ConfigureAwait(false);
        }

        protected override bool ValidateEntity(T entity)
        {
            if (entity != null && entity.Id >= 0)
            {
                return true;
            }

            LogHelper.LogException<BaseDataRepository>("Invalid entity: null or empty primary keys");
            return false;
        }

        #endregion
    }
}
