

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class AddVenueReviewDto
    {
        public int VenueID { get; set; }
        public string ReviewerID { get; set; }
        public string Review { get; set; }
        public int Rating { get; set; }
        public bool Recommend { get; set; }
    }
}
