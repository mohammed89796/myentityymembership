using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.FacilityDtos;

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class VenueWrapperDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public string VenueLocation { get; set; }
        public string VenueLongitude { get; set; }
        public string VenueLatitude { get; set; }

       
        public FacilityWrapperDto Facility { get; set; }
        public BranchWrapperDto Branch { get; set; }
    }

    public class  GetVenueWrapperDto
    {

        public VenueWrapperDto Venue { get; set; }
    }
}
