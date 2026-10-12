using System.Linq.Expressions;

namespace MyEntityMembershipService.SpecificationPattern
{
    public abstract class Specifications<T> where T : class
    {
        protected Specifications(Expression<Func<T, bool>>? criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>>? Criteria { get; }
        // public List<Expression<Func<T, object>>> IncludeExpressions { get; } = new List<Expression<Func<T, object>>>();

        public List<Func<IQueryable<T>, IQueryable<T>>> Includes { get; set; } = new List<Func<IQueryable<T>, IQueryable<T>>>();
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        // Pagination
        public int Skip { get; private set; }
        public int Take { get; private set; }
        public bool IsPaginated { get; private set; }

        public bool AsNoTracking { get; set; }
        public bool AsSplitQuery { get; set; }
        //public void AddInclude(Expression<Func<T, object>> expression)
        //    => IncludeExpressions.Add(expression);

        public void AddInclude(Func<IQueryable<T>, IQueryable<T>> includeExpression)
    => Includes.Add(includeExpression);
        public void SetOrderBy(Expression<Func<T, object>> orderExpression)
            => OrderBy = orderExpression;
        public void SetOrderByDescending(Expression<Func<T, object>> orderExpression)
            => OrderByDescending = orderExpression;

        protected void ApplyPagination(int PageIndex, int PageSize)
        {
            IsPaginated = true;
            Take = PageSize;
            Skip = (PageIndex - 1) * PageSize;
        }
    }
}
