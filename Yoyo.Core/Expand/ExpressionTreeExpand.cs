using System;
using System.Linq.Expressions;

namespace Yoyo.Core.Expand
{
    /// <summary>
    /// 表达式拓展
    /// </summary>
    public static class ExpressionTreeExpand
    {
        #region Expression<Func<T, bool>>表达式树方法
        /// <summary>
        /// &合并表达式
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="exp1">表达式1</param>
        /// <param name="exp2">表达式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            if (exp1 == null) { return exp2; }
            if (exp2 == null) { return exp1; }
            return exp1.Compose<T>(exp2, Expression.And);
        }
        /// <summary>
        /// &&合并表达式
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="exp1">表达式1</param>
        /// <param name="exp2">表达式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            if (exp1 == null) { return exp2; }
            if (exp2 == null) { return exp1; }
            return exp1.Compose<T>(exp2, Expression.AndAlso);
        }
        /// <summary>
        /// |合并表达式
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="exp1">表达式1</param>
        /// <param name="exp2">表达式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            if (exp1 == null) { return exp2; }
            if (exp2 == null) { return exp1; }
            return exp1.Compose<T>(exp2, Expression.Or);
        }
        /// <summary>
        /// ||合并表达式
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="exp1">表达式1</param>
        /// <param name="exp2">表达式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            if (exp1 == null) { return exp2; }
            if (exp2 == null) { return exp1; }
            return exp1.Compose<T>(exp2, Expression.OrElse);
        }
        private static Expression<Func<T, bool>> Compose<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, Func<Expression, Expression, BinaryExpression> func)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(func(left, right), parameter);
        }
        #endregion

        #region 拓展方法
        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue) { return _newValue; }
                return base.Visit(node);
            }
        }
        #endregion
    }
}
