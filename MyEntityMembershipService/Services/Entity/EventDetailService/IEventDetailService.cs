using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.EventDetailService
{
    public interface IEventDetailService
    {
        Task<Response<EventDetailToReturnDto>> CreateEventDetail(EventDetailDto eventDetailDto);

        Task<Response<EventDetailToReturnDto>> UpdateEventDetail(UpdateEventDetailDto eventDetailDto, int eventDetailsId);

        Task<Response<EventDetailToReturnDto>> DeleteEventDetail(int eventDetailId);

        Task<Response<EventDetailToReturnDto>> GetEventDetailById(int eventDetailId);

        Task<Response<IReadOnlyList<EventDetailToReturnDto>>> GetAllEventDetail();

    }
}
