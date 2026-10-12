using MyEntityMembershipService.DTOs.Membership.FormDtos;

namespace MyEntityMembershipService.DTOs.Membership.ParentApplicationFormDtos
{
    public class ParentApplicationFormFilterDto
    {
        public int? TenantId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ParentApplicationFormDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Alias { get; set; }
        public double? Fees { get; set; }
        public string? WorkflowSelection { get; set; }
        public bool IsDraft { get; set; }
        public List<int>? MainMemberTypeIds { get; set; } = [];
        public List<int>? ChildMemberTypeIds { get; set; } = [];
        public List<FormFieldsDto> ParentFieldsDto { get; set; } = [];
        public List<FormFieldsDto> ChildFieldsDto { get; set; } = [];
        public string? UserName { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
