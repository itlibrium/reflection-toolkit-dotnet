using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace ITLIBRIUM.Reflection
{
    public static class ExpressionAttributeExtensions
    {
        private const string InvalidExpressionError = "Expression should be method, constructor, property or field";

        [PublicAPI]
        public static TAttribute GetAttribute<TAttribute>(this LambdaExpression expression, bool inherit = true)
            where TAttribute : Attribute =>
            expression.Body switch
            {
                MethodCallExpression methodCallExp => methodCallExp.Method.GetCustomAttribute<TAttribute>(inherit),
                MemberExpression memberExp => memberExp.Member.GetCustomAttribute<TAttribute>(inherit),
                NewExpression constructorExp => constructorExp.Constructor.GetCustomAttribute<TAttribute>(inherit),
                _ => throw new ArgumentException(InvalidExpressionError, nameof(expression))
            };
    }
}