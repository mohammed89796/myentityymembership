

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenuePaginationListDto
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public List<VenueCardDetailsDto> Venues { get; set; }
    }

    public class VenueCardDetailsDto
    {
        public int VenueID { get; set; }
        public string ImageURL { get; set; }
        public string VenueName { get; set; }
        public string VenueType { get; set; }
        public double Rating { get; set; }
    }
}
