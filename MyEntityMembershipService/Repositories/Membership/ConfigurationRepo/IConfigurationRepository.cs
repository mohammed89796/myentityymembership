using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.ConfigurationDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.ConfigurationRepo
{
	public interface IConfigurationRepository : IGenericRepository<PackageConfigurationSection>
	{
		Task<List<ConfigurationResponseDto>> GetPackageDetailsByRequestTypeAndPackageID(int requestTypeId, int packageID);
		Task<Configuration> GetConfigeWithFields(int Id);

    }
}
