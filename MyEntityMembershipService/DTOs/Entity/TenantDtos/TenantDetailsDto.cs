using MyEntityMembershipService.DTOs.Entity.PackageDtos;

namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class TenantDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }

        public List<SocialMediaDto> SocialMediaLinks { get; set; } = new List<SocialMediaDto>();
        public TenantDetailsDto? SuperEntity { get; set; }

        public int? FedrationEntityTypeId { get; set; }
        public int? EntityTypeId { get; set; }
        public int? FedrationBranchId { get; set; }

        public List<PackageDto> Packages { get; set; } = new List<PackageDto>();

        public bool IsBlocked { get; set; }
    }
}
