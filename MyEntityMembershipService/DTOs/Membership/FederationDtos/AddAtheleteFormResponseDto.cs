namespace MyEntityMembershipService.DTOs.Membership.FederationDtos
{
    public class AddAtheleteFormResponseDto
    {
        public int? BatchId { get; set; }
        public int TenentId { get; set; }
        public int? FormId { get; set; }
        public string? TenentName { get; set; }
        public int? SuperTenentId { get; set; }
        public string? SuperTenentName { get; set; }
        public int? PackageId { get; set; }
        public int? PackagePrice { get; set; }
        public List<AtheleteFieldResponseDto> FieldsResponse { get; set; }
       
    }
}
