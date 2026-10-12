

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenueReviewDto
    {
        public VenueReviewStarRatingsDto StarRating { get; set; }
        public List<VenueReviewsDto> Reviews { get; set; }
    }

    public class VenueReviewStarRatingsDto
    {
        public double AverageRatings { get; set; }
        public double RecommendPercentage { get; set; }
        public int FiveStar { get; set; }
        public int FourStar { get; set; }
        public int ThreeStar { get; set; }
        public int TwoStar { get; set; }
        public int OneStar { get; set; }
    }
    public class VenueReviewsDto
    {
        public int StarRating { get; set; }
        public string ReviewerName { get; set; }
        public string Review { get; set; }
        public DateTime? ReviewDate { get; set; }
    }

    public class GetVenueDashboardReviewDto
    {
        public int TotalPages { get; set; }
        public List<VenueReviewsDto> Reviews { get; set; }
    }
}
