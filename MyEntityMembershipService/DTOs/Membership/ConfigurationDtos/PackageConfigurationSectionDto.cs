

namespace MyEntityMembershipService.DTOs.Membership.ConfigurationDtos
{
    public class PackageConfigurationSectionDto
    {
        public string Name { get; set; }
        public int ConfigurationId { get; set; }
        public List<ConfigurationFieldDto> ConfigurationFields { get; set; }
    }
}
