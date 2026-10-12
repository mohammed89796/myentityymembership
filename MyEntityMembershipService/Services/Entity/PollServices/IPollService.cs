using MyEntityMembershipService.DTOs.Entity.VoteDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.PollServices
{
    public interface IPollService
    {
        Task<Response<object>> VoteOnAPoll(VoteDto pollDto);
    }
}
