using MyEntityMembershipService.DTOs.Membership.FormDtos;

namespace MyEntityMembershipService.DTOs.Membership.ParentApplicationFormDtos
{
    public class CreateParentApplicationFormDto
    {
        public string Title { get; set; }
        public string? Alias { get; set; }
        public double Fees { get; set; }
        public string WorkflowSelection { get; set; }
        public int RequestTypeId { get; set; }
        public List<int> MemberTypesId { get; set; } = [];
        public List<FormFieldsDto> ParentFieldsDto { get; set; } = [];
        public List<FormFieldsDto> ChildFieldsDto { get; set; } = [];
        public List<CustomParentApplicationFormFieldsDto>? ParentCustomFields { get; set; }
        public List<CustomParentApplicationFormFieldsDto>? ChildCustomFields { get; set; }
        public bool IsDraft { get; set; } = true;
        public int? TenantId { get; set; }
    }



    public class CustomParentApplicationFormFieldsDto
    {
        public string Title { get; set; }
        public string TitleAr { get; set; }
        public string MapField { get; set; }
        public int FieldTypeId { get; set; }
        public int? UnitTypeId { get; set; }
        public bool IsAdminFilled { get; set; }
        public bool IsRequired { get; set; }
        public List<string>? DropdownValues { get; set; }

    }
}
