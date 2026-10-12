using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.VenueReviewService;

namespace MyEntityMembershipService.Controllers.V1.Entity.Venue
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class VenueReviewsController : ControllerBase
    {
        private readonly IVenueReviewService _VenueReviewsService;

        public VenueReviewsController(IVenueReviewService venueReviewsService)
        {
            _VenueReviewsService = venueReviewsService;
        }

        [HttpPost("CreateReview")]
        public async Task<IResult> CreateReview(AddVenueReviewDto review)

            => (await _VenueReviewsService.CreateReview(review)).GetResponse();

        [HttpGet("GetVenueReviews/{venueID}")]
        public async Task<IResult> GetVenueReviews(int venueID)

            => (await _VenueReviewsService.GetVenueReviews(venueID)).GetResponse();
        [HttpGet("GetVenueReviewsDashBoard")]
        public async Task<IResult> GetVenueReviewsDashBoard(int venueID, int pageNumber)

            => (await _VenueReviewsService.GetVenueReviewsDashBoard(venueID, pageNumber)).GetResponse();

    }
}
