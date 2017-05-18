using System;
using System.Linq.Expressions;

namespace DataLayer.Extensions.Linq
{
    public static class EntityFrameworkLinqExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> firstExpression, Expression<Func<T, bool>> secondExpression)
        {
            ParameterExpression parameter;
            Expression left, right;

            GetVisitorExpressions(firstExpression, secondExpression, out parameter, out left, out right);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> firstExpression, Expression<Func<T, bool>> secondExpression)
        {
            ParameterExpression parameter;
            Expression left, right;

            GetVisitorExpressions(firstExpression, secondExpression, out parameter, out left, out right);

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
        }

        private static void GetVisitorExpressions<T>(Expression<Func<T, bool>> firstExpression,
            Expression<Func<T, bool>> secondExpression, out ParameterExpression parameter, out Expression left, out Expression right)
        {
            parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(firstExpression.Parameters[0], parameter);
            left = leftVisitor.Visit(firstExpression.Body);

            var rightVisitor = new ReplaceExpressionVisitor(secondExpression.Parameters[0], parameter);
            right = rightVisitor.Visit(secondExpression.Body);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression mNewValue;
            private readonly Expression mOldValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                mOldValue = oldValue;
                mNewValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                return node == mOldValue ? mNewValue : base.Visit(node);
            }
        }
    }
}
