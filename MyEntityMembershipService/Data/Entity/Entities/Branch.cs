namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }

        public ICollection<Facility>? Facilities { get; set; } = new HashSet<Facility>();
        public ICollection<NewFacility>? NewFacilities { get; set; } = new HashSet<NewFacility>();
        public ICollection<AccessedBranches>? AccessedBranches { get; set; } = new HashSet<AccessedBranches>();
        public ICollection<AccessedBranchesFacilities>? AccessedBranchesFacilities { get; set; } = new HashSet<AccessedBranchesFacilities>();
        public ICollection<AccessedFacilitiesVenues>? AccessedFacilitiesVenues { get; set; } = new HashSet<AccessedFacilitiesVenues>();

        //Newly added
        public string BranchImageURL { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        public int PostalCode { get; set; }
        public string BranchManagerID { get; set; }
        public string Description { get; set; }
        public virtual BranchLocation BranchLocation { get; set; }
        public virtual BranchOperatingHours BranchOperatingHours { get; set; }
        public ICollection<BranchImages> BranchImages { get; set; }
        public ICollection<BranchServices> BranchServices { get; set; } = new HashSet<BranchServices>();

        //Contact information
        public string CountryCodeID { get; set; }
        public int PhoneNumber { get; set; }
        public string EmailAddress { get; set; }


    }
}
