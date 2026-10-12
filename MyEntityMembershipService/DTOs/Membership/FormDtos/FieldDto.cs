

namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class FieldDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? TitleAr { get; set; }
        public int FieldTypeId { get; set; }
        public string FieldTypeName { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsEditable { get; set; }
        public int FieldCategoryId { get; set; }
        public bool? IsChecked { get; set; }
        public string? DefaultValue { get; set; }
        public bool? IsHidden { get; set; }
        public List<string>? DropdownValuesDto { get; set; }
        public List<FieldUnitsDto> FieldUnits { get; set; }
    }
}
