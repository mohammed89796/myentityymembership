using Microsoft.EntityFrameworkCore;

namespace MyEntityMembershipService.SpecificationPattern
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(this Specifications<T> Spec, IQueryable<T> inputQuery) where T : class
        {
            var query = inputQuery;
            // Filter
            if (Spec.Criteria is not null)
                query = query.Where(Spec.Criteria);
            // Include
            //query = Spec.IncludeExpressions.Aggregate(query,
            //     (currentQuery, includeExpression) => currentQuery.Include(includeExpression)
            //    );

            query = Spec.Includes.Aggregate(query, (current, includeFunc) => includeFunc(current));


            // Order
            if (Spec.OrderBy is not null)
                query = query.OrderBy(Spec.OrderBy);
            else if (Spec.OrderByDescending is not null)
                query = query.OrderByDescending(Spec.OrderByDescending);
            // Pagination
            if (Spec.IsPaginated)
                query = query.Skip(Spec.Skip).Take(Spec.Take);

            if (Spec.AsNoTracking)
                query = query.AsNoTracking();

            if (Spec.AsSplitQuery)

                query = query.AsSplitQuery();

            return query;
        }
    }
}
