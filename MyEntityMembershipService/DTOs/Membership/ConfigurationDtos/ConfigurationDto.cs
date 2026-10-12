
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;

namespace MyEntityMembershipService.DTOs.Membership.ConfigurationDtos
{
    public class ConfigurationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PackageConfigurationSectionDto> PackageConfigurationSections { get; set; } = new List<PackageConfigurationSectionDto>();
        public List<ConfigrationFieldResponseDto>? ConfigrationFieldResponses { get; set; } = new List<ConfigrationFieldResponseDto>();

    }
}
