using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs
{
    public class EntityResults
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string VenueName { get; set; }
        public string TenantName { get; set; }
        public string BranchName { get; set; }
        public string FacilityName { get; set; }
    }

    public class FederationResults
    {
        public int Id { get; set; }
        public string FedImage { get; set; }
        public string FedName { get; set; }
        public string Location { get; set; }
    }

    public class SportsClubsResults
    {
        public int Id { get; set; }
        public string ClubImage { get; set; }
        public string ClubName { get; set; }
        public string Location { get; set; }
    }
    
    public class PackagesResults
    {
        public int Id { get; set; }
        public string PackageImage { get; set; }
        public string PackageName { get; set; }
        public double Price { get; set; }
        public string EntityName { get; set; }
    }

}
