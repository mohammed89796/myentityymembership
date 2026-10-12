

namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class GetFacilityMobileDetailsDto
    {
        public FacilityHeaderMobile FacilityHeader { get; set; }
        public FacilityAboutMobile FacilityAbout { get; set; }
        public List<string> Sports { get; set; } = new List<string>();
        public List<string> Accessibility { get; set; } = new List<string>();
        public List<string> Amenities { get; set; } = new List<string>();
        public NewFacilityOperatingHours OperatingHours { get; set; }
        public List<string> ImageUrls { get; set; }
    }
    public class FacilityHeaderMobile
    {
        public string FacilityImageUrl { get; set; }
        public string FacilityName { get; set; }
        public string BranchName { get; set; }

    }
    public class FacilityAboutMobile
    {
        public string BranchImage { get; set; }
        public string BranchName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Country { get; set; }
        public string Governorate { get; set; }


    }
}
