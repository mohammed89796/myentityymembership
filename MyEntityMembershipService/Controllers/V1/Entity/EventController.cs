using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.EventService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }


        [HttpPost("AddEvent")]
        public async Task<IResult> CreateEvent([FromBody] EventDto eventDto)

            => (await _eventService.CreateEvent(eventDto)).GetResponse();


        [HttpPut("UpdateEvent/{eventId}")]
        public async Task<IResult> UpdateEvent([FromBody] UpdateEventDto eventDto, [FromRoute] int eventId)

            => (await _eventService.UpdateEvent(eventDto, eventId)).GetResponse();


        [HttpDelete("DeleteEvent/{eventId}")]
        public async Task<IResult> DeleteEvent(int eventId)

            => (await _eventService.DeleteEvent(eventId)).GetResponse();


        [HttpGet("GetEvent")]
        public async Task<IResult> GetEvent(int eventId)

            => (await _eventService.GetEventById(eventId)).GetResponse();


        [HttpGet("GetAllEvents")]
        public async Task<IResult> GetAllEvents()

            => (await _eventService.GetAllEvents()).GetResponse();


        [HttpGet("GetAllEventsWithPagination")]
        public async Task<IResult> GetAllEventsWithPagination([FromQuery] int seriesId, [FromQuery] int pageNumber, [FromQuery] int pageSize)

            => (await _eventService.GetAllEventsWithPagination(seriesId, pageSize, pageNumber)).GetResponse();


        [HttpPost("FilterEventsWithPagination/{seriesId}")]
        public async Task<IResult> FilterEventsWithPagination([FromRoute] int seriesId, [FromBody] FilterEventDto filterEventDto, [FromQuery] int pageNumber, [FromQuery] int pageSize)

            => (await _eventService.FilterEvents(seriesId, filterEventDto, pageSize, pageNumber)).GetResponse();


        [HttpGet("SortEventsWithPagination/{seriesId}")]
        public async Task<IResult> SortEventsWithPagination([FromRoute] int seriesId, [FromQuery] string sort, [FromQuery] int pageNumber, [FromQuery] int pageSize)

            => (await _eventService.SortEvents(seriesId, sort, pageSize, pageNumber)).GetResponse();
    }
}
