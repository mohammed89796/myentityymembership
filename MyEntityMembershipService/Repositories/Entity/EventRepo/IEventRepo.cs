
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Repositories.Entity.EventRepo
{
    public interface IEventRepo : IGenericRepository<Event>
    {
        Task<Event> GetByIdWithEventDetailAsync(int id);

        Task<List<Event>> GetAllWithEventDetailAsync();

        Task<List<Event>> GetAllWithEventDetailAsync(int seriesId, int pageNumber, int pageSize);

        Task<int> GetCountOfEventsAsync(int seriesId);

        Task<int> GetSumOfPriceOfAllEventsAsync(int seriesId);

        Task<List<Event>> FilterEventsAsync(Expression<Func<Event, bool>> criteria, int pageNumber, int pageSize);

        Task<int> GetCountOfFilteredEventsAsync(Expression<Func<Event, bool>> criteria);

        Task<List<Event>> SortEventsAsync(int seriesId, Expression<Func<Event, object>> criteria, int pageNumber, int pageSize);

        Task<List<Event>> SortEventsDescAsync(int seriesId, Expression<Func<Event, object>> criteria, int pageNumber, int pageSize);
    }
}
