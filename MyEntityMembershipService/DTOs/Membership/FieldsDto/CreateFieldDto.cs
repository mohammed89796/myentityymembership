namespace MyEntityMembershipService.DTOs.Membership.FieldsDto
{
    public class CreateFieldDto
    {
        public string Title { get; set; }
        public string MapField { get; set; }
        public int FieldTypeId { get; set; }
        public int? UnitTypeId { get; set; }
    }
}
