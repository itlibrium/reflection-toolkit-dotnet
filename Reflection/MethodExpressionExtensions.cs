using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace ITLIBRIUM.Reflection
{
    public static class MethodExpressionExtensions
    {
        private const string InvalidExpressionError = "Expression should be method";

        [PublicAPI]
        public static IEnumerable<Type> GetParameters(this LambdaExpression lambdaExp)
        {
            var bodyExp = lambdaExp.Body;
            if (bodyExp.TryGetMethodCallExpression(out var methodCallExp))
                return methodCallExp.Method.GetParameters().Select(p => p.ParameterType);

            throw new ArgumentException(InvalidExpressionError, nameof(lambdaExp));
        }
    }
}