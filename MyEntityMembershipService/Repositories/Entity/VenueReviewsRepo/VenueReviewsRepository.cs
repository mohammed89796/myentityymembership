using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;


using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueReviewsRepo
{
    public class VenueReviewsRepository : GenericRepository<VenueReviews>, IVenueReviewsRepository
    {
        private readonly EntityMembershipDbContext _context;

        public VenueReviewsRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<GetVenueReviewDto> GetVenueReviewsAsync(int venueId)
        {
            var reviews = await _context.VenueReviews
                .Where(vr => vr.VenueID == venueId)
                .OrderByDescending(vr => vr.CreationDate)
                .ToListAsync();

            var starRatings = new VenueReviewStarRatingsDto
            {
                AverageRatings =  reviews.Any() ? reviews.Average(r => r.Rating) : 0,
                RecommendPercentage = reviews.Any() ? reviews.Count(r => r.Recommend) / (double)reviews.Count() * 100 : 0,
                FiveStar =  reviews.Any() ? reviews.Count(r => r.Rating == 5) :0,
                FourStar =  reviews.Any() ?reviews.Count(r => r.Rating == 4):0,
                ThreeStar = reviews.Any() ? reviews.Count(r => r.Rating == 3):0,
                TwoStar =  reviews.Any() ?reviews.Count(r => r.Rating == 2):0,
                OneStar =  reviews.Any() ? reviews.Count(r => r.Rating == 1):0
            };

            var reviewList = reviews.Select(r => new VenueReviewsDto
            {
                StarRating = r.Rating,
                ReviewerName = "Mo7sen",
                Review = r.Review,
                ReviewDate = r.CreationDate
            }).ToList();

            return new GetVenueReviewDto
            {
                StarRating = starRatings,
                Reviews = reviewList
            };
        }
        public async Task<GetVenueDashboardReviewDto> GetVenueReviewsDashBoardAsync(int venueId, int pageNumber)
        {
            int pageSize = 10;

            var totalReviews = await _context.VenueReviews
                .Where(vr => vr.VenueID == venueId)
                .CountAsync();

            var totalPages = (int)Math.Ceiling(totalReviews / (double)pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageNumber = pageNumber > totalPages ? totalPages : pageNumber;

            if (totalReviews == 0)
            {
                return new GetVenueDashboardReviewDto
                {
                    TotalPages = 0,
                    Reviews = new List<VenueReviewsDto>()
                };
            }

            var reviews = await _context.VenueReviews
                .Where(vr => vr.VenueID == venueId)
                .OrderByDescending(vr => vr.CreationDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var reviewList = reviews.Select(r => new VenueReviewsDto
            {
                StarRating = r.Rating,
                ReviewerName = "Mo7sen", //FIXME: Replace with actual logic if dynamic
                Review = r.Review,
                ReviewDate = r.CreationDate
            }).ToList();

            return new GetVenueDashboardReviewDto
            {
                TotalPages = totalPages,
                Reviews = reviewList
            };
        }
        public async Task<bool> CheckForPastReviews(int venueId, string reviewerID)
        {
            var review = await _context.VenueReviews.FirstOrDefaultAsync(r => r.VenueID == venueId && r.ReviewerID == reviewerID);
            return review != null;
        }



    }
}
