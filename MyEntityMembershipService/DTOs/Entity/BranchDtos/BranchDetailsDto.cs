

namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class BranchDetailsDto
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string BranchName { get; set; }
        public string Description { get; set; }
        public int NumberOfFacilities { get; set; }
        public List<BranchImages> BranchImages { get; set; }
        public BranchInformationDto BranchInformation { get; set; }
        public BranchLocationDetailsDto LocationDetails { get; set; }
        public BranchContactInfoDto ContactInfo { get; set; }
        public BranchOperatingHours OperatingHours { get; set; }

    }
    public class BranchInformationDto
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public int PostalCode { get; set; }
        public string BranchManagerName { get; set; }
        public List<string> BranchServices { get; set; }
    }
    public class BranchLocationDetailsDto
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class BranchContactInfoDto
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
