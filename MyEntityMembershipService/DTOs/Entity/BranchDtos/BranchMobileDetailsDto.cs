namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class BranchMobileDetailsDto
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public BranchMobileAboutDto BranchAboutView { get; set; }
        public BranchMobileInformationDto BranchInformationView { get; set; }
        public BranchMobileLocationDetailsDto LocationDetails { get; set; }
        public List<BranchMobileMediaDto> BranchMedia { get; set; }
    }
    public class BranchMobileAboutDto
    {
        public string Description { get; set; }
        public string Governorate { get; set; }
        public int PostalCode { get; set; }
        public string BranchManagerName { get; set; }
        public List<string> BranchServices { get; set; }
    }
    public class BranchMobileInformationDto
    {
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
    public class BranchMobileLocationDetailsDto
    {
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class BranchMobileMediaDto
    {
        public int ID { get; set; }
        public string ImageURL { get; set; }
    }
}
