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
            var methodCallExp = expression.Body as MethodCallExpression;
            if (methodCallExp != null)
                return methodCallExp.Method.GetCustomAttribute<TAttribute>(inherit);

            var memberExp = expression.Body as MemberExpression;
            if (memberExp != null)
                return memberExp.Member.GetCustomAttribute<TAttribute>(inherit);

            var constructorExp = expression.Body as NewExpression;
            if (constructorExp != null)
                return constructorExp.Constructor.GetCustomAttribute<TAttribute>(inherit);

            throw new ArgumentException(InvalidExpressionError, nameof(expression));
        }
    }
}