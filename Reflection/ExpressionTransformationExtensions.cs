using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace ITLIBRIUM.Reflection
{
    public static class ExpressionTransformationExtensions
    {
        private const string InvalidExpressionError =
            "Expression should be property with public setter or not readonly field";

        [PublicAPI]
        public static Action<TSource, TValue> CreateSetter<TSource, TValue>(
            this Expression<Func<TSource, TValue>> getter)
        {
            if (!TryCreateSetter(getter, out var setter))
                throw new ArgumentException(InvalidExpressionError, nameof(getter));
            return setter;
        }

        [PublicAPI]
        public static bool TryCreateSetter<TSource, TValue>(this Expression<Func<TSource, TValue>> getter,
            out Action<TSource, TValue> setter)
        {
            if (!(getter.Body is MemberExpression memberExp))
                throw new ArgumentException(InvalidExpressionError, nameof(getter));

            if (!CanSet(memberExp.Member))
            {
                setter = null;
                return false;
            }

            var sourceExp = Expression.Parameter(typeof(TSource));
            var valueExp = Expression.Parameter(typeof(TValue));
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

        private static bool CanSet(MemberInfo memberInfo) =>
            memberInfo switch
            {
                PropertyInfo propertyInfo => (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsPublic),
                FieldInfo fieldInfo => !fieldInfo.IsInitOnly,
                _ => false
            };
    }
}