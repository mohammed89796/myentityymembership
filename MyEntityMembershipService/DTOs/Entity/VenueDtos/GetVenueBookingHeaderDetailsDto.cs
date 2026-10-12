


namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenueBookingHeaderDetailsDto
    {
        public string VenueImageURL { get; set; }
        public string VenueName { get; set; }
        public string BranchName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string PhoneNumber { get; set; }
        public GetVenueBookingFEaturesDetailsDto Features { get; set; }

    }

    public class GetVenueBookingFEaturesDetailsDto
    {
        public string Capacity { get; set; }
        public string Dimensions { get; set; }
        public string Type { get; set; }
        public string TurfTypes { get; set; }
        public List<string> Utilities { get; set; }
        public List<string> Accessibility { get; set; }
        public List<string> Amenities { get; set; }

    }


}
