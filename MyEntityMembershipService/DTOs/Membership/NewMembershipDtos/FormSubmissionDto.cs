using MyEntityMembershipService.DTOs.Membership.FederationDtos;
using MyEntityMembershipService.DTOs.Membership.FormDtos;

namespace MyEntityMembershipService.DTOs.Membership.NewMembershipDtos
{
    public class FormSubmissionDto
    {
        public int TenantId { get; set; }
        public string? TenantName { get; set; }
        public int PackageId { get; set; }
        public int FormId { get; set; }
        public int? BatchId { get; set; }
        public string UserId { get; set; }
        public List<AtheleteFieldResponseDto> FieldsResponses { get; set; } = new();
        public List<FormSubmissionDto> Children { get; set; } = new();
    }
}
