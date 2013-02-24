using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WebApi.Models;
using WebApi.Queryable;

namespace WebApi
{
    public class MarcQueryable<T> : IOrderedQueryable<T>
    {
        public MarcQueryable()
        {
            Provider = new MarcQueryProvider();
            Expression = Expression.Constant(this);
        }

        public MarcQueryable(IQueryProvider provider, Expression expression)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (expression == null) throw new ArgumentNullException("expression");

            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Provider
                .Execute<IEnumerable<T>>(Expression)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get; private set; }
        public IQueryProvider Provider { get; private set; }

        public Type ElementType
        {
            get { return typeof(T); }
        }
    }

    public class MarcQueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            return CreateQuery<Book>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MarcQueryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            // Find the call to Where() and get the lambda expression predicate.
            InnermostWhereFinder whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);

            if (whereExpression == null) 
            {
                return (TResult)Enumerable.Empty<Book>();
            }

            LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            // Send the lambda expression through the partial evaluator.
            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);
            ParameterFinder<Book> lf = new ParameterFinder<Book>(lambdaExpression.Body);
            var parameters = lf.Parameters;

            return (TResult)((IEnumerable<Book>)new SearchOperations().GetBooksBy(parameters).ToArray());
        }
    }
}