using MyEntityMembershipService.DTOs.Membership.FormDtos;

namespace MyEntityMembershipService.DTOs.Membership.ParentApplicationFormDtos
{
    public class UpdateParentApplicationFormDto
    {
        public int Id { get; set; } 
        public int? TenantId { get; set; }
        public string? Title { get; set; }
        public string? Alias { get; set; }
        public double? Fees { get; set; }
        public string? WorkflowSelection { get; set; }
        public bool? IsDraft { get; set; }
        public List<int>? MemberTypesId { get; set; }
        public List<FormFieldsDto>? ParentFieldsDto { get; set; }
        public List<FormFieldsDto>? ChildFieldsDto { get; set; }
        public List<CustomParentApplicationFormFieldsDto>? ParentCustomFields { get; set; }
    }
}
