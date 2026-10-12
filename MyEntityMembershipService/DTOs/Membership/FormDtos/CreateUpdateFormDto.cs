namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class CreateUpdateFormDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Alias { get; set; }
        public double? Fees { get; set; }
        public string? WorkflowSelection { get; set; }
        public List<FormFieldsDto> FieldsDto { get; set; }
        public int RequestTypeId { get; set; }
        public string? CreatedBy { get; set; }
        public List<int>? MemberTypesId { get; set; } = new List<int>();
        public bool IsDraft { get; set; } = true;
        public int? TenantId { get; set; }
        public List<CustomFormFieldsDto>? CustomFormFieldsDtos { get; set; }

    }


    public class CustomFormFieldsDto
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
