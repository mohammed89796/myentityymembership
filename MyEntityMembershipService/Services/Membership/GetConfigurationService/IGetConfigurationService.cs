using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.GetConfigurationService
{
    public interface IGetConfigurationService
    {
        Task<Response<object>> GetConfiguartionAsync(int packageId, int RequestTypesId, int? subMemberId = null);
    }
}
