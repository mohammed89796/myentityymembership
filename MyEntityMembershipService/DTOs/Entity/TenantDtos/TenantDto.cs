using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;

namespace MyEntityMembershipService.DTOs.Entity.TenantDtos
{
    public class TenantDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Website { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public List<BranchDto> Branches { get; set; } = new List<BranchDto>();
        public List<PackageDto> Packages { get; set; } = new List<PackageDto>();

        public bool IsBlocked { get; set; }


    }
}
