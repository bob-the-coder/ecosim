using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Interfaces;

namespace DataLayer.Repositories
{
    public abstract class BaseRepository<T> : GenericDataRepository<T>
        where T : class, IDataAccessObject, new()
    {
        #region Public methods

        public virtual async Task<T> CreateAsync(T entity, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            if (!ValidateEntity(entity))
            {
                return null;
            }

            entity = await AddAsync(entity).ConfigureAwait(false);

            return refreshFromDb ? await FetchFromDbAsync(entity, navigationProperties).ConfigureAwait(false) : entity;
        }

        public virtual async Task<IList<T>> CreateAsync(IList<T> entities, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            if (entities.Any(entity => !ValidateEntity(entity)))
            {
                return null;
            }

            entities = await AddAsync(entities).ConfigureAwait(false);

            if (!refreshFromDb)
            {
                return entities;
            }

            var refreshedEntities = new List<T>();
            foreach (var entity in entities)
            {
                var refreshedEntity = await FetchFromDbAsync(entity, navigationProperties).ConfigureAwait(false);

                refreshedEntities.Add(refreshedEntity);
            }

            return refreshedEntities;
        }

        public virtual async Task<T> UpdateAsync(T entity, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            if (!ValidateEntity(entity))
            {
                return null;
            }

            entity = await ChangeAsync(entity).ConfigureAwait(false);

            return refreshFromDb ? await FetchFromDbAsync(entity, navigationProperties).ConfigureAwait(false) : entity;
        }

        public virtual async Task<IList<T>> UpdateAsync(IList<T> entities, bool refreshFromDb = false, IList<string> navigationProperties = null)
        {
            if (entities.Any(entity => !ValidateEntity(entity)))
            {
                return null;
            }

            entities = await ChangeAsync(entities).ConfigureAwait(false);

            if (!refreshFromDb)
            {
                return entities;
            }

            var refreshedEntities = new List<T>();
            foreach (var entity in entities)
            {
                var refreshedEntity = await FetchFromDbAsync(entity, navigationProperties).ConfigureAwait(false);

                refreshedEntities.Add(refreshedEntity);
            }

            return refreshedEntities;
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            if (!ValidateEntity(entity))
            {
                return false;
            }

            await RemoveAsync(entity).ConfigureAwait(false);

            return true;
        }

        public virtual async Task<bool> DeleteAsync(IList<T> entities)
        {
            if (entities.Any(entity => !ValidateEntity(entity)))
            {
                return false;
            }

            await RemoveAsync(entities).ConfigureAwait(false);

            return true;
        }

        #endregion

        #region Abstract methods 

        protected abstract Task<T> FetchFromDbAsync(T entity, IList<string> navigationProperties = null);

        protected abstract bool ValidateEntity(T entity);

        #endregion
    }
}
