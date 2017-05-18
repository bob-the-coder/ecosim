using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using BusinessLogic.Workflow.Interfaces;
using BusinessLogic.TypeManagement;
using DataLayer;
using DataLayer.Interfaces;

namespace BusinessLogic.Workflow
{
    public class OrderedConfigurableQuery<TEntity, TModel> : IOrderedConfigurableQuery<TEntity, TModel>
        where TEntity : class, IDataAccessObject
        where TModel : class, IModel
    {
        private readonly Entities mContext;
        private IOrderedQueryable<TEntity> mDbQuery;

        public OrderedConfigurableQuery()
        {
        }

        public OrderedConfigurableQuery(Entities context, IOrderedQueryable<TEntity> dbQuery)
        {
            mContext = context;
            mDbQuery = dbQuery;
        }

        public bool IsValid => mContext != null && mDbQuery != null;

        public IOrderedConfigurableQuery<TEntity, TModel> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            if (IsValid)
            {
                mDbQuery = mDbQuery.OrderBy(expression);
            }

            return this;
        }

        public IOrderedConfigurableQuery<TEntity, TModel> OrderByDesc<TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            if (IsValid)
            {
                mDbQuery = mDbQuery.OrderByDescending(expression);
            }

            return this;
        }

        public IOrderedConfigurableQuery<TEntity, TModel> ThenBy<TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            if (IsValid)
            {
                mDbQuery = mDbQuery.ThenBy(expression);
            }

            return this;
        }

        public IOrderedConfigurableQuery<TEntity, TModel> ThenByDesc<TKey>(Expression<Func<TEntity, TKey>> expression)
        {
            if (IsValid)
            {
                mDbQuery = mDbQuery.ThenByDescending(expression);
            }

            return this;
        }

        public IConfigurableQuery<TEntity, TModel> Paginate(int pageNumber, int numberOfItemsPerPage)
        {
            if (!IsValid)
            {
                return this;
            }

            return new ConfigurableQuery<TEntity, TModel>(mContext, mDbQuery.Skip(pageNumber * numberOfItemsPerPage).Take(numberOfItemsPerPage));
        }

        public IList<TModel> Execute()
        {
            return mDbQuery.ToList().CopyTo<TModel>();
        }

        public async Task<IList<TModel>> ExecuteAsync()
        {
            var result = await mDbQuery.ToListAsync().ConfigureAwait(false);
            return result.CopyTo<TModel>();
        }

        public void Dispose()
        {
            mContext?.Dispose();
        }
    }
}
