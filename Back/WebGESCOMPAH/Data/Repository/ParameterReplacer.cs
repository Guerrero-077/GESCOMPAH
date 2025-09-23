using System.Linq.Expressions;

namespace Data.Repository
{
    public class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly Expression _to;

        private ParameterReplacer(ParameterExpression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        public static Expression Replace(Expression expression, ParameterExpression from, Expression to)
        {
            return new ParameterReplacer(from, to).Visit(expression)!;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _from ? _to : base.VisitParameter(node);
        }
    }

}
