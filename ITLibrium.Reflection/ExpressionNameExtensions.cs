using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ITLibrium.Reflection
{
    public static class ExpressionNameExtensions
    {
        private const string InvalidExpressionError = "Expression should be method, property or field";

        public static string GetName(this LambdaExpression lambdaExp)
        {
            Expression bodyExp = lambdaExp.Body;
            if (bodyExp.TryGetMemberExpression(out MemberExpression memberExp))
                return GetFieldOrPropertyName(memberExp);

            if (bodyExp.TryGetMethodCallExpression(out MethodCallExpression methodCallExp))
                return methodCallExp.Method.Name;

            throw new ArgumentException(InvalidExpressionError, nameof(lambdaExp));
        }

        public static string GetPath(this LambdaExpression lambdaExp)
        {
            var elements = new List<string>(5);
            Expression currentExp = lambdaExp.Body;
            while (true)
            {
                if (currentExp.TryGetMemberExpression(out MemberExpression memberExp))
                {
                    string element = GetFieldOrPropertyName(memberExp);
                    elements.Add(element);
                    currentExp = memberExp.Expression;
                }
                else if (currentExp.TryGetMethodCallExpression(out MethodCallExpression methodCallExp))
                {
                    string element = methodCallExp.Method.Name;
                    elements.Add(element);
                    currentExp = methodCallExp.Object;
                }
                else
                {
                    break;
                }
            }

            var builder = new StringBuilder();
            for (int i = elements.Count - 1; i > 0; i--)
            {
                builder.Append(elements[i]);
                builder.Append('.');
            }
            builder.Append(elements[0]);

            return builder.ToString();
        }

        private static string GetFieldOrPropertyName(MemberExpression memberExp)
        {
            MemberInfo memberInfo = memberExp.Member;

            if (memberInfo is PropertyInfo propertyInfo)
                return propertyInfo.Name;

            if (memberInfo is FieldInfo fieldInfo)
                return fieldInfo.Name;

            throw new ArgumentException(InvalidExpressionError, nameof(memberExp));
        }
    }
}