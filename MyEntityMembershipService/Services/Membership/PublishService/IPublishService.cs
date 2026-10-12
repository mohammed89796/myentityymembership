using MyEntityMembershipService.DTOs.Membership.UserDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.PublishService
{
    public interface IPublishService
    {
        Task<Response<string>> PublishUserData(UserDataDto userDataDto);
    }
}
