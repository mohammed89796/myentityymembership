namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class FieldResponseDto
    {
        public string? Response { get; set; }
        public int FieldId { get; set; }
        public string? FieldName { get; set; }
        public string? FieldTypeName { get; set; }
        public int? UnitId { get; set; }
    }
}
