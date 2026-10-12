
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.PollRepo
{
    public interface IPollRepository : IGenericRepository<Poll>
    {
        Task<Poll> GetPollWithRelatedDataByIdAsync(int pollId, string userId);
    }
}
