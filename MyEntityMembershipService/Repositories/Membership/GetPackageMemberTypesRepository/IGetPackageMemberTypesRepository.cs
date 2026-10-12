using MyEntityMembershipService.DTOs.Membership.GetPackageMemberTypesDtos;

namespace MyEntityMembershipService.Repositories.Membership.GetPackageMemberTypesRepository
{
    public interface IGetPackageMemberTypesRepository
    {
        public Task<List<int>> GetPackageMemberTypeIdsAsync(int packageId);
        public Task<List<GetPackageMemberTypesDto>> GetMemberTypeNamesByIdsAsync(List<int> memberTypeIds);
    }
}
