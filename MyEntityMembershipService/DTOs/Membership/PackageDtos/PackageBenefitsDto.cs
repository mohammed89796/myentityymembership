

using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
{


    namespace MyEntityMembershipService.DTOs.Membership.PackageDtos
    {
        public class PackageBenefitsDto
        {
            public ICollection<PackageAccess> PackageAccesses { get; set; } = [];
            public ICollection<PackageEligibility> PackageEligibilities { get; set; } = [];
        }

        public class PackageEligibility
        {
            public int ProductId { get; set; }
            public ProductType ProductType { get; set; }
            public string Name { get; set; } = null!;
            public string? Image { get; set; }

            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class PackageAccess
        {
            public int BranchId { get; set; }
            public string BranchName { get; set; } = null!;      
            public string? BranchImage { get; set; } 
            public ICollection<PackageFacilityAccess> PackageFacilityAccesses { get; set; } = [];
        }

        public class PackageFacilityAccess
        {
            public int FacilityId { get; set; }
            public string FacilityName { get; set; } = null!;   
            public string? FacilityImage { get; set; }            
            public ICollection<PackageVenuesAccess> PackageVenuesAccesses { get; set; } = [];
        }

        public class PackageVenuesAccess
        {
            public int VenueId { get; set; }
            public string VenueName { get; set; } = null!;     
            public string? VenueImage { get; set; }  
        }
    }


}
