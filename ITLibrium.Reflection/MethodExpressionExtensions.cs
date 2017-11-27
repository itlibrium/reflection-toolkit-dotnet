using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ITLibrium.Reflection
{
    public static class MethodExpressionExtensions
    {
        private const string InvalidExpressionError = "Expression should be method";

        public static IEnumerable<Type> GetParameters(this LambdaExpression lambdaExp)
        {
            Expression bodyExp = lambdaExp.Body;
            if (bodyExp.TryGetMethodCallExpression(out MethodCallExpression methodCallExp))
                return methodCallExp.Method.GetParameters().Select(p => p.ParameterType);

            throw new ArgumentException(InvalidExpressionError, nameof(lambdaExp));
        }
    }
}