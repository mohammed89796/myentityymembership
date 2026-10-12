namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class Field : BaseEntity
    {
        public string Title { get; set; }
        public string? TitleAr { get; set; }
        public string MapField { get; set; }
        public int FieldTypeId { get; set; }
        public FieldType FieldType { get; set; }
        public int? UnitTypeId { get; set; }
        public UnitType UnitType { get; set; }
        public List<DropdownValue>? DropdownValues { get; set; }
        public List<FieldResponse> FieldResponses { get; set; }
        public List<RequestTypeFields> RequestTypeFields { get; set; }
        public List<FormFields> FormFields { get; set; }
    }
}
