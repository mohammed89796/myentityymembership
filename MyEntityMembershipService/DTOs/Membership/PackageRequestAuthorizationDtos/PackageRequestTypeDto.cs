
using MyEntityMembershipService.DTOs.Membership.ConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.FormDtos;

namespace MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos
{
    public class PackageRequestTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ConfigurationDto Configuration { get; set; }
        public List<PackageFormDto> PackageForms { get; set; }

    }
}
