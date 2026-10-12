

using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.VenueReviewService
{
    public interface IVenueReviewService
    {
        Task<Response<bool>> CreateReview(AddVenueReviewDto review);
        Task<Response<GetVenueReviewDto>> GetVenueReviews(int venueID);
        Task<Response<GetVenueDashboardReviewDto>> GetVenueReviewsDashBoard(int venueID, int pageNumber);
    }
}
