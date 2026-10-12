using MyEntityMembershipService.DTOs.Membership.FederationDtos;

namespace MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos
{
    public class ExcelAtheleteRowDto
    {
        public int TenentId { get; set; }
        public string? TenentName { get; set; }
        public int? SuperTenentId { get; set; }
        public string? SuperTenentName { get; set; }
        public int? PackageId { get; set; }
        public int? PackagePrice { get; set; }
        public int? BatchId { get; set; }
        public IFormFile file { get; set; }


        public List<AtheleteFieldResponseDto>? FieldsResponse { get; set; } = new();
    }

}
