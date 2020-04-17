using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace ITLIBRIUM.ReflectionToolkit
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

        [PublicAPI]
        public static IEnumerable<object> GetParameterValues(this LambdaExpression lambdaExp)
        {
            if (!TryGetParameterValues(lambdaExp, out var values))
                throw new NotSupportedException($"Can not extract parameters values from {lambdaExp}");
            return values;
        }

        [PublicAPI]
        public static bool TryGetParameterValues(this LambdaExpression lambdaExp, out IEnumerable<object> values)
        {
            var bodyExp = lambdaExp.Body;
            if (!bodyExp.TryGetMethodCallExpression(out var methodCallExp))
                throw new ArgumentException(InvalidExpressionError, nameof(lambdaExp));

            var count = methodCallExp.Arguments.Count;
            if (count == 0)
            {
                values = Enumerable.Empty<object>();
                return true;
            }

            var valuesArray = new object[count];
            for (var i = 0; i < count; i++)
            {
                var exp = methodCallExp.Arguments[i];
                if (!TryGetExpressionValue(exp, out var value))
                {
                    values = null;
                    return false;
                }
                valuesArray[i] = value;
            }
            values = valuesArray;
            return true;
        }

        private static bool TryGetExpressionValue(Expression exp, out object value)
        {
            switch (exp)
            {
                case ConstantExpression constantExp:
                    value = constantExp.Value;
                    return true;
                case NewExpression newExp:
                    return TryGetExpressionValue(newExp, out value);
                case MemberExpression memberExp:
                    return TryGetExpressionValue(memberExp, out value);
                case MethodCallExpression methodCallExp:
                    return TryGetExpressionValue(methodCallExp, out value);
                default:
                    value = null;
                    return false;
            }
        }

        private static bool TryGetExpressionValue(NewExpression newExp, out object value)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - it can be null for structs
            // ReSharper disable once HeuristicUnreachableCode
            if (newExp.Constructor is null)
            {
                // ReSharper disable once HeuristicUnreachableCode
                value = FormatterServices.GetSafeUninitializedObject(newExp.Type);
                return true;
            }

            if (!TryGetArguments(newExp.Arguments, out var arguments))
            {
                value = null;
                return false;
            }
            value = newExp.Constructor.Invoke(arguments);
            return true;
        }

        private static bool TryGetExpressionValue(MemberExpression memberExp, out object value)
        {
            if (!TryGetExpressionValue(memberExp.Expression, out var obj))
            {
                value = null;
                return false;
            }

            switch (memberExp.Member)
            {
                case FieldInfo fieldInfo:
                    value = fieldInfo.GetValue(obj);
                    return true;
                case PropertyInfo propertyInfo:
                    value = propertyInfo.GetValue(obj);
                    return true;
                default:
                    value = null;
                    return false;
            }
        }

        private static bool TryGetExpressionValue(MethodCallExpression methodCallExp, out object value)
        {
            if (!TryGetArguments(methodCallExp.Arguments, out var arguments))
            {
                value = null;
                return false;
            }

            object obj;
            if (methodCallExp.Object is null)
            {
                obj = null;
            }
            else if (!TryGetExpressionValue(methodCallExp.Object, out obj))
            {
                value = null;
                return false;
            }
            value = methodCallExp.Method.Invoke(obj, arguments);
            return true;
        }

        private static bool TryGetArguments(IReadOnlyList<Expression> argumentExps, out object[] arguments)
        {
            var count = argumentExps.Count;
            arguments = new object[count];
            for (var i = 0; i < count; i++)
            {
                if (!TryGetExpressionValue(argumentExps[i], out var argument))
                {
                    arguments = null;
                    return false;
                }
                arguments[i] = argument;
            }
            return true;
        }
    }
}