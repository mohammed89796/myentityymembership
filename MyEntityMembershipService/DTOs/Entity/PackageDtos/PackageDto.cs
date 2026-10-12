

using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.FacilityDtos;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;

namespace MyEntityMembershipService.DTOs.Entity.PackageDtos
{
    public class PackageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public DurationUnit DurationUnit { get; set; }
        public double Price { get; set; }
        public int TenantId { get; set; }
        public string? TenantName { get; set; }
        public string? TenantImage { get; set; }

        public int SubMembersLimit { get; set; }
        public string? Image { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public bool? IsSubscribed { get; set; }

        public List<SubMemberCategoryDto>? SubMemberCategories { get; set; } = new List<SubMemberCategoryDto>();
        public List<BranchDto>? AcssedBranches { get; set; } = new List<BranchDto>();
        public List<NewFacilityDto> Facilities { get; set; }=new List<NewFacilityDto>();
        public List<NewVenueDto> Venues { get; set; } = new List<NewVenueDto>();
        public List<PackageAuthourizationsDto>? PackageAuthourizations { get; set; } = new List<PackageAuthourizationsDto>();

    }
}
