
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.EventDetailRepo
{
    public interface IEventDetailRepo : IGenericRepository<EventDetail>
    {
        Task<EventDetail> GetByIdWithEventAsync(int id);

        Task<List<EventDetail>> GetAllWithEventAsync();
    }
}
