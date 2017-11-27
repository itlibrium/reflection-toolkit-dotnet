using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ITLibrium.Reflection
{
    public static class ExpressionAttributeExtensions
    {
        private const string InvalidExpressionError = "Expression should be method, constructor, property or field";

        public static TAttribute GetAttribute<TAttribute>(this LambdaExpression expression, bool inherit = true)
            where TAttribute : Attribute
        {
            if (expression.Body is MethodCallExpression methodCallExp)
                return methodCallExp.Method.GetCustomAttribute<TAttribute>(inherit);

            if (expression.Body is MemberExpression memberExp)
                return memberExp.Member.GetCustomAttribute<TAttribute>(inherit);

            if (expression.Body is NewExpression constructorExp)
                return constructorExp.Constructor.GetCustomAttribute<TAttribute>(inherit);

            throw new ArgumentException(InvalidExpressionError, nameof(expression));
        }
    }
}