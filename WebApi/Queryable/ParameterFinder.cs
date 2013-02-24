using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace WebApi.Queryable
{
    internal class ParameterFinder<T> : ExpressionVisitor
    {
        private readonly Expression _expression;
        private List<KeyValuePair<string, string>> _pairs;

        public ParameterFinder(Expression exp)
        {
            _expression = exp;
        }

        public IEnumerable<KeyValuePair<string, string>> Parameters
        {
            get
            {
                if (_pairs == null)
                {
                    _pairs = new List<KeyValuePair<string, string>>();
                    Visit(_expression);
                }
                return _pairs;
            }
        }

        protected override Expression VisitBinary(BinaryExpression be)
        {
            Type bookType = typeof(T);
            List<string> fields = bookType.GetProperties().Select(s => s.Name).ToList();

            if (be.NodeType == ExpressionType.Equal)
            {
                foreach (var field in fields)
                {
                    if (!ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(T), field)) continue;

                    var pair = new KeyValuePair<string, string>(
                        field,
                        ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(T), field));

                    _pairs.Add(pair);
                    return be;
                }
                return base.VisitBinary(be);
            }
            return base.VisitBinary(be);
        }
    }
}