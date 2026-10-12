using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.FormUserResponseRepo
{
    public interface IFormUserResponseRepository :IGenericRepository<FormUserResponse>
    {
        Task<FormUserResponse> GetFormUserResponse(int Id);
        Task<FormUserResponse> GetResponseByRequestId(int RequestId);
        Task<FormUserResponse> GetFormUserResponseWithForm(int RequestId);
    }
}
