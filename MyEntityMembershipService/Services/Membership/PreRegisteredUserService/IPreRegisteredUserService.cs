using MyEntityMembershipService.DTOs.Membership.PreRegisteredUsersDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.PreRegisteredUserService
{
    public interface IPreRegisteredUserService
    {
        Task<Response<PreRegisteredUserToReturnDto>> GetPreRegisteredUserByFileNumberAsync(string fileNumber);
    }
}
