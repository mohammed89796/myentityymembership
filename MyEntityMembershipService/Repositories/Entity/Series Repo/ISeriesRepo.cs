
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Linq.Expressions;

namespace MyEntityMembershipService.APi.Repositories.Series_Repo
{
    public interface ISeriesRepo : IGenericRepository<Series>
    {
        Task<Series> GetByIdWithEventAndEventDetailAsync(int id);

        Task<IReadOnlyList<Series>> GetAllWithEventAndEventDetailAsync(Expression<Func<Series, bool>> criteria);

        Task<IReadOnlyList<Series>> FilterSeriesAsync(Expression<Func<Series, bool>> criteria);

        Task<IReadOnlyList<Series>> SortSeriesAsync(Expression<Func<Series, object>> criteria);

        Task<IReadOnlyList<Series>> SortSeriesDescendingAsync(Expression<Func<Series, object>> criteria);
    }
}
