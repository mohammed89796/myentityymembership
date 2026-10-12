namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class ReturnDraftFormDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Alias { get; set; }
        public double? Fees { get; set; }
        public string? WorkflowSelection { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public string? CreatedBy { get; set; }
        public string? UserName { get; set; } // Created By User Name
        public DateTime? CreationDate { get; set; }
        public List<DraftFieldsGroupedByCategory> CategoriesFields { get; set; }

        public List<DraftMemberTypeDto> MemberTypes { get; set; }
        public bool IsDraft { get; set; }
    }
    public class DraftFieldsGroupedByCategory
    {
        public int FieldCategoryId { get; set; }
        public string FieldCategoryName { get; set; }
        public List<DraftFieldDto> Fields { get; set; }
    }
    public class DraftFieldDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int FieldTypeId { get; set; }
        public string FieldTypeName { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsEditable { get; set; }
        public int FieldCategoryId { get; set; }
        public bool? IsChecked { get; set; }
        public string? DefaultValue { get; set; }
        public List<string>? DropdownValuesDto { get; set; }
        public List<FieldUnitsDto> FieldUnits { get; set; }
        public bool WasSelected { get; set; } = false;
    }
    public class DraftMemberTypeDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public bool WasSelected { get; set; } = false;

    }

}
