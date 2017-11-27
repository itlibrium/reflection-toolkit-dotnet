using System.Linq.Expressions;

namespace ITLibrium.Reflection
{
    internal static class ExpressionExtensions
    {
        public static bool TryGetMemberExpression(this Expression exp, out MemberExpression memberExp)
        {
            exp = SkipCastExpression(exp);
            memberExp = exp as MemberExpression;
            return memberExp != null;
        }

        public static bool TryGetMethodCallExpression(this Expression exp, out MethodCallExpression methodCallExp)
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
    }
}