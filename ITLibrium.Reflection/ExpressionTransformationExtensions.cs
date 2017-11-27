using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ITLibrium.Reflection
{
    public static class ExpressionTransformationExtensions
    {
        private const string InvalidExpressionError = "Expression should be property with public setter or not readonly field";

        public static Action<TSource, TValue> CreateSetter<TSource, TValue>(this Expression<Func<TSource, TValue>> getter)
        {
            if (!TryCreateSetter(getter, out Action<TSource, TValue> setter))
                throw new ArgumentException(InvalidExpressionError, nameof(getter));

            return setter;
        }

        public static bool TryCreateSetter<TSource, TValue>(this Expression<Func<TSource, TValue>> getter, out Action<TSource, TValue> setter)
        {
            if (!(getter.Body is MemberExpression memberExp))
                throw new ArgumentException(InvalidExpressionError, nameof(getter));

            if (!CanSet(memberExp.Member))
            {
                setter = null;
                return false;
            }

            ParameterExpression sourceExp = Expression.Parameter(typeof(TSource));
            ParameterExpression valueExp = Expression.Parameter(typeof(TValue));

            setter = Expression.Lambda<Action<TSource, TValue>>(
                Expression.Assign(
                    Expression.MakeMemberAccess(
                        sourceExp,
                        memberExp.Member),
                    valueExp),
                sourceExp,
                valueExp).Compile();

            return true;
        }

        private static bool CanSet(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    return propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsPublic;
                case FieldInfo fieldInfo:
                    return !fieldInfo.IsInitOnly;
                default:
                    return false;
            }
        }
    }
}