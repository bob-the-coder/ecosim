using System;
using System.Linq.Expressions;
using BusinessLogic.Interfaces;
using DataLayer.Interfaces;

namespace BusinessLogic.Workflow.Interfaces
{
    public interface IOrderedConfigurableQuery<TEntity, TModel> : IConfigurableQuery<TEntity, TModel>
        where TEntity : class, IDataAccessObject
        where TModel : class, IModel
    {
        IOrderedConfigurableQuery<TEntity, TModel> ThenBy<TKey>(Expression<Func<TEntity, TKey>> expression);

        IOrderedConfigurableQuery<TEntity, TModel> ThenByDesc<TKey>(Expression<Func<TEntity, TKey>> expression);
    }
}
