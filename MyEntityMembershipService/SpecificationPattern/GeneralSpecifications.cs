using System.Linq.Expressions;

namespace MyEntityMembershipService.SpecificationPattern
{
    public class GeneralSpecifications<T> : Specifications<T> where T : class
    {
        public GeneralSpecifications() : base(null) { }
        public GeneralSpecifications(int PageIndex , int PageSize) : base(null)
        {
            ApplyPagination(PageIndex, PageSize);
        }
        public GeneralSpecifications(Expression<Func<T, bool>> expression) : base(expression) { }
        public GeneralSpecifications(Expression<Func<T, bool>> expression, int PageIndex, int PageSize) : base(expression)
        {
            ApplyPagination(PageIndex, PageSize);
        }

    }
}
