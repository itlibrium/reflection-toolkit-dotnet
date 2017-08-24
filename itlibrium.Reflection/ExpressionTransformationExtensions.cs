using System;
using System.Linq.Expressions;
using System.Reflection;

namespace itlibrium.Reflection
{
    public static class ExpressionTransformationExtensions
    {
        private const string InvalidExpressionError = "Expression should be property with public setter or not readonly field";

        public static Action<TSource, TValue> CreateSetter<TSource, TValue>(this Expression<Func<TSource, TValue>> getter)
        {
            Action<TSource, TValue> setter;
            if (!TryCreateSetter(getter, out setter))
                throw new ArgumentException(InvalidExpressionError, nameof(getter));

            return setter;
        }

        public static bool TryCreateSetter<TSource, TValue>(this Expression<Func<TSource, TValue>> getter, out Action<TSource, TValue> setter)
        {
            var memberExp = getter.Body as MemberExpression;
            if (memberExp == null)
                throw new ArgumentException(InvalidExpressionError, nameof(getter));

            var propertyInfo = memberExp.Member as PropertyInfo;
            var fieldInfo = memberExp.Member as FieldInfo;
            if ((propertyInfo?.SetMethod == null || !propertyInfo.SetMethod.IsPublic) && (fieldInfo == null || fieldInfo.IsInitOnly))
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
    }
}