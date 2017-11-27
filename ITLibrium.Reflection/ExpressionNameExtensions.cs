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

        public static string GetName(this LambdaExpression expression)
        {
            if (TryGetMemberExpression(expression.Body, out MemberExpression memberExp))
                return GetFieldOrPropertyName(memberExp);

            if (TryGetMethodCallExpression(expression.Body, out MethodCallExpression methodCallExp))
                return methodCallExp.Method.Name;

            throw new ArgumentException(InvalidExpressionError, nameof(expression));
        }

        public static string GetPath(this LambdaExpression expression)
        {
            var elements = new List<string>(5);
            Expression currentExp = expression.Body;
            while (true)
            {
                if (TryGetMemberExpression(currentExp, out MemberExpression memberExp))
                {
                    string element = GetFieldOrPropertyName(memberExp);
                    elements.Add(element);
                    currentExp = memberExp.Expression;
                }
                else if (TryGetMethodCallExpression(currentExp, out MethodCallExpression methodCallExp))
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

        private static bool TryGetMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            exp = SkipCastExpression(exp);

            memberExp = exp as MemberExpression;
            return memberExp != null;
        }

        private static bool TryGetMethodCallExpression(Expression exp, out MethodCallExpression methodCallExp)
        {
            exp = SkipCastExpression(exp);

            methodCallExp = exp as MethodCallExpression;
            return methodCallExp != null;
        }

        private static Expression SkipCastExpression(Expression exp)
        {
            if (exp.NodeType != ExpressionType.Convert && exp.NodeType != ExpressionType.ConvertChecked)
                return exp;

            return exp is UnaryExpression castExp ? castExp.Operand : exp;
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