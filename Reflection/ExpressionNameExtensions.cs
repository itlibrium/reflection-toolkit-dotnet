using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace ITLIBRIUM.Reflection
{
    public static class ExpressionNameExtensions
    {
        private const string InvalidExpressionError = "Expression should be method, property or field";

        [PublicAPI]
        public static string GetName(this LambdaExpression lambdaExp)
        {
            var bodyExp = lambdaExp.Body;
            if (bodyExp.TryGetMemberExpression(out var memberExp))
                return GetFieldOrPropertyName(memberExp);

            if (bodyExp.TryGetMethodCallExpression(out var methodCallExp))
                return methodCallExp.Method.Name;

            throw new ArgumentException(InvalidExpressionError, nameof(lambdaExp));
        }

        [PublicAPI]
        public static string GetPath(this LambdaExpression lambdaExp)
        {
            var elements = new List<string>(5);
            var currentExp = lambdaExp.Body;
            while (true)
            {
                if (currentExp.TryGetMemberExpression(out var memberExp))
                {
                    var element = GetFieldOrPropertyName(memberExp);
                    elements.Add(element);
                    currentExp = memberExp.Expression;
                }
                else if (currentExp.TryGetMethodCallExpression(out var methodCallExp))
                {
                    var element = methodCallExp.Method.Name;
                    elements.Add(element);
                    currentExp = methodCallExp.Object;
                }
                else
                {
                    break;
                }
            }

            var builder = new StringBuilder();
            for (var i = elements.Count - 1; i > 0; i--)
            {
                builder.Append(elements[i]);
                builder.Append('.');
            }
            builder.Append(elements[0]);

            return builder.ToString();
        }

        private static string GetFieldOrPropertyName(MemberExpression memberExp) =>
            memberExp.Member switch
            {
                PropertyInfo propertyInfo => propertyInfo.Name,
                FieldInfo fieldInfo => fieldInfo.Name,
                _ => throw new ArgumentException(InvalidExpressionError, nameof(memberExp))
            };
    }
}