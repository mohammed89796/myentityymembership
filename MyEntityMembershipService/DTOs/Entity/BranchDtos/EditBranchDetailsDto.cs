

namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class EditBranchDetailsDto
    {
        public int ID { get; set; }
        public string? BranchImageURL { get; set; }
        public string BranchName { get; set; }
        public BranchCountryDetailsDto AddressDetails { get; set; }
        public string Location { get; set; }
        public int PostalCode { get; set; }
        public string? BranchManagerID { get; set; }
        public string? BranchManagerName { get; set; }
        public List<BranchServicesDetailsDto> BranchServices { get; set; }
        public string Description { get; set; }
        public EditBranchContactInfoDto ContactInformation { get; set; }
        public EditBranchLocationDto BranchLocation { get; set; }
        public EditBranchMediaDto BranchMedia { get; set; }
        public BranchOperatingHours BranchOperatingHours { get; set; }
    }
    public class BranchServicesDetailsDto
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
    }
    public class BranchCountryDetailsDto
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; }
    }
    public class EditBranchContactInfoDto
    {
        public int CountryCodeID { get; set; }
        public string CountryCode { get; set; }
        public int PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
    public class EditBranchLocationDto
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class EditBranchMediaDto
    {
        public int ID { get; set; }
        public string Link { get; set; }
    }
}
