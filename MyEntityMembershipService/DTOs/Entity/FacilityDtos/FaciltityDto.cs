
using MyEntityMembershipService.DTOs.Entity.VenueDtos;

namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class FaciltityDto
    {
        public int Id { get; set; }
        public string Image { get; set; }

        public string name { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int BranchId { get; set; }
        public int? FacilityTypeId { get; set; }
        public FacilityType? FacilityType { get; set; }
        public List<AddVenueDto>? Venues { get; set; }
        public List<SportsAccommodatedDto>? FacilitySportsAccommodated { get; set; }

        public UpdateFacilityOperatingHoursDto? FacilityOperatingHours { get; set; }
    }
}
