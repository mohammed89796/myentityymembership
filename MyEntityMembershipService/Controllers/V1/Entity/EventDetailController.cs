using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.Services.Entity.EventDetailService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class EventDetailController : ControllerBase
    {
        private readonly IEventDetailService _eventDetailService;

        public EventDetailController(IEventDetailService eventDetailService)
        {
            _eventDetailService = eventDetailService;
        }

        [HttpPost("AddEventDetail")]
        public async Task<IResult> CreateEventDetail([FromForm] EventDetailDto eventDetailDto)

            => (await _eventDetailService.CreateEventDetail(eventDetailDto)).GetResponse();


        [HttpPut("UpdateEventDetail/{eventDetailsId}")]
        public async Task<IResult> UpdateEventDetail([FromForm] UpdateEventDetailDto eventDetailDto, [FromRoute] int eventDetailsId)

            => (await _eventDetailService.UpdateEventDetail(eventDetailDto, eventDetailsId)).GetResponse();


        [HttpDelete("DeleteEventDetail/{eventDetailsId}")]
        public async Task<IResult> DeleteEventDetail(int eventDetailsId)

            => (await _eventDetailService.DeleteEventDetail(eventDetailsId)).GetResponse();


        [HttpGet("GetEventDetail/{eventDetailsId}")]
        public async Task<IResult> GetEventDetail(int eventDetailsId)

            => (await _eventDetailService.GetEventDetailById(eventDetailsId)).GetResponse();


        [HttpGet("GetAllEventDetails")]
        public async Task<IResult> GetAllEventDetails()

            => (await _eventDetailService.GetAllEventDetail()).GetResponse();
    }
}
