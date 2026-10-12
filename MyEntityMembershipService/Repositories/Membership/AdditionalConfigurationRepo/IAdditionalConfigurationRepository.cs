using Elastic.CommonSchema;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.AdditionalConfigurationRepo
{
    public interface IAdditionalConfigurationRepository : IGenericRepository<AdditionalConfiguration>
    {

        Task<AdditionalConfiguration> GetAdditionalConfigurationWithSubMemeberByIdAsync(int Id, int packageId);
        Task<double?> GetAdditionFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null);
    }
}
