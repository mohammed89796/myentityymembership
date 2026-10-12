namespace MyEntityMembershipService.DTOs.Membership.FederationDtos
{
    public class AtheleteFieldResponseDto
    {
        public string? Response { get; set; }
        public IFormFile? File { get; set; }
        public int FieldId { get; set; }
        public string? FieldName { get; set; }
        public string? FieldTypeName { get; set; }
        public int? UnitId { get; set; }
    }
}
