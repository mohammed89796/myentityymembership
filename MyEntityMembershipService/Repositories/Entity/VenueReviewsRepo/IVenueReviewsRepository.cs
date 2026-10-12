

using MyEntityMembershipService.DTOs.Entity.VenueDtos;

using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.VenueReviewsRepo
{
    public interface IVenueReviewsRepository : IGenericRepository<VenueReviews>
    {
        Task<GetVenueReviewDto> GetVenueReviewsAsync(int venueId);
        Task<GetVenueDashboardReviewDto> GetVenueReviewsDashBoardAsync(int venueId, int pageNumber);
        Task<bool> CheckForPastReviews(int venueId, string reviewerID);
    }
}
