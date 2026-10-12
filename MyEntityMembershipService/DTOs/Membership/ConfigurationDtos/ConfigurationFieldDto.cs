
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;

namespace MyEntityMembershipService.DTOs.Membership.ConfigurationDtos
{
    public class ConfigurationFieldDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? FieldUnit { get; set; }

        public FieldTypeDto FieldType { get; set; }

        public List<ConfigDropDownValuesDto> configDropDownValuesDtos { get; set; }=new List<ConfigDropDownValuesDto>();

    }
}
