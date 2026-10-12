using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.EventService
{
    public interface IEventService
    {
        Task<Response<EventToReturnDto>> CreateEvent(EventDto eventDto);

        Task<Response<EventToReturnDto>> UpdateEvent(UpdateEventDto eventDto, int eventId);

        Task<Response<EventToReturnDto>> DeleteEvent(int eventId);

        Task<Response<EventToReturnDto>> GetEventById(int eventId);

        Task<Response<IReadOnlyList<EventToReturnDto>>> GetAllEvents();

        Task<Response<PaginationToReturnDto<EventToReturnDto>>> GetAllEventsWithPagination(int seriesId, int pageSize, int pageNumber);

        Task<Response<PaginationToReturnDto<EventToReturnDto>>> FilterEvents(int seriesId, FilterEventDto filters, int pageSize = 6, int pageNumber = 1);

        Task<Response<PaginationToReturnDto<EventToReturnDto>>> SortEvents(int seriesId, string sort, int pageSize, int pageNumber);

    }
}
