
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.DTOs.Membership.RequestTypeDtos;

namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class PackageFormDto
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string? WorkflowSelection { get; set; }
        public double? Fees { get; set; }
        public RequestTypeDto RequestType { get; set; }
        public List<FieldDto> Fields { get; set; }
        public PaakageMemberFormDto PaakageMemberForms {  get; set; }
    }
}
