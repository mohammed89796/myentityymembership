using MyEntityMembershipService.DTOs.Membership.GetPackageMemberTypesDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.GetPackageMemberTypesService
{
    public interface IGetPackageMemberTypesService
    {

        public Task<Response<List<int>>> GetPackageMemberTypeIdsAsync(int packageId);
        public Task<Response<List<GetPackageMemberTypesDto>>> GetMemberTypeNamesByIdsAsync(int packageId);
    }
}
