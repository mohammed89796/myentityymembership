using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;


using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.Entity.VenueReviewsRepo;


namespace MyEntityMembershipService.Services.Entity.VenueReviewService
{
    public class VenueReviewService : IVenueReviewService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IVenueReviewsRepository _venueReviewsRepository;


        public VenueReviewService(IMapper mapper, IUnitOfWork unitOfWork, IVenueReviewsRepository venueReviewsRepository, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _venueReviewsRepository = venueReviewsRepository;
        }

        public async Task<Response<bool>> CreateReview(AddVenueReviewDto review)
        {
            try
            {
                var pastReview = await _venueReviewsRepository.CheckForPastReviews(review.VenueID, review.ReviewerID);
               
                if (pastReview)
                {
                    return new ResponseHandler().BadRequest<bool>("You have already posted your feedback.");
                    
                }
                
                var reviewDetails = new VenueReviews
                {
                    VenueID = review.VenueID,
                    ReviewerID = review.ReviewerID,
                    Rating = review.Rating,
                    Review = review.Review,
                    Recommend = review.Recommend,
                    CreationDate = DateTime.UtcNow
                };

                await _unitOfWork.Repository<VenueReviews>().Add(reviewDetails);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }

        public async Task<Response<GetVenueReviewDto>> GetVenueReviews(int venueID)
        {
            try
            {
                var venueReviews = await _venueReviewsRepository.GetVenueReviewsAsync(venueID);

                if (venueReviews is null)
                {
                    return new ResponseHandler().NotFound<GetVenueReviewDto>("Venue reviews not found");
                }

                return new ResponseHandler().Success(venueReviews);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetVenueReviewDto>("Something went wrong");
            }
        }
        public async Task<Response<GetVenueDashboardReviewDto>> GetVenueReviewsDashBoard(int venueID, int pageNumber)
        {
            try
            {
                var venueReviews = await _venueReviewsRepository.GetVenueReviewsDashBoardAsync(venueID, pageNumber);

                if (venueReviews is null)
                {
                    return new ResponseHandler().NotFound<GetVenueDashboardReviewDto>("Venue reviews not found");
                }

                return new ResponseHandler().Success(venueReviews);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetVenueDashboardReviewDto>("Something went wrong");
            }
        }
    }
}
