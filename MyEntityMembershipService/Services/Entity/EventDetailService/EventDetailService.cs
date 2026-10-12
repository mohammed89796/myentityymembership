using AutoMapper;

using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.EventDetailRepo;
using MyEntityMembershipService.Repositories.Entity.EventRepo;

namespace MyEntityMembershipService.Services.Entity.EventDetailService
{
    public class EventDetailService : IEventDetailService
    {

        private readonly IEventDetailRepo _eventDetailRepo;
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public EventDetailService(IEventDetailRepo eventDetailRepo, IEventRepo eventRepo, IMapper mapper, IConfiguration configuration)
        {

            _eventDetailRepo = eventDetailRepo;
            _eventRepo = eventRepo;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<EventDetailToReturnDto>> CreateEventDetail(EventDetailDto eventDetailDto)
        {
            try
            {

                var eventOfEventDetail = await _eventRepo.GetByIdAsync(eventDetailDto.EventId);

                if (eventOfEventDetail is null)
                    return new ResponseHandler().BadRequest<EventDetailToReturnDto>("invalid eventId");

                var eventDetailPictureUrl = string.Empty;

                if (eventDetailDto.EventDetailPicture is not null)
                {
                    var validationResult = ValidateOnEventDetailDto(eventDetailDto);

                    if (validationResult.Length > 0)
                        return new ResponseHandler().BadRequest<EventDetailToReturnDto>(validationResult);

                    var pictureUri = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", eventDetailDto.EventDetailPicture);

                    eventDetailPictureUrl = pictureUri.ToString();


                }

                var mappedEventDetail = _mapper.Map<EventDetailDto, EventDetail>(eventDetailDto);

                mappedEventDetail.EventDetailPictureUrl = eventDetailPictureUrl;

                await _eventDetailRepo.Add(mappedEventDetail);

                await _eventDetailRepo.SaveAsync();

                var mappedEventDetailToReturn = _mapper.Map<EventDetail, EventDetailToReturnDto>(mappedEventDetail);

                return new ResponseHandler().Created(mappedEventDetailToReturn);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EventDetailToReturnDto>("Something went wrong");

            }
        }


        public async Task<Response<EventDetailToReturnDto>> UpdateEventDetail(UpdateEventDetailDto eventDetailDto, int eventDetailsId)
        {
            try
            {
                var eventOfEventDetail = await _eventRepo.GetByIdAsync(eventDetailDto.EventId);

                if (eventOfEventDetail is null)
                    return new ResponseHandler().BadRequest<EventDetailToReturnDto>("invalid eventId");

                var eventDetail = await _eventDetailRepo.GetByIdAsync(eventDetailsId);

                if (eventDetail is null)
                    return new ResponseHandler().NotFound<EventDetailToReturnDto>("invalid eventDetailsId");

                var eventDetailPictureUrl = string.Empty;

                if (eventDetailDto.EventDetailPicture is not null)
                {

                    var validationResult = ValidateOnUpdateEventDetailDto(eventDetailDto);

                    if (validationResult.Length > 0)
                        return new ResponseHandler().BadRequest<EventDetailToReturnDto>(validationResult);

                    var pictureUri = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", eventDetailDto.EventDetailPicture);

                    eventDetailPictureUrl = pictureUri.ToString();

                }



                eventDetail.Duration = string.IsNullOrEmpty(eventDetailDto.Duration) ? eventDetail.Duration : eventDetailDto.Duration;
                eventDetail.Title = string.IsNullOrEmpty(eventDetailDto.Title) ? eventDetail.Title : eventDetailDto.Title;
                eventDetail.Description = string.IsNullOrEmpty(eventDetailDto.Description) ? eventDetail.Description : eventDetailDto.Description;
                eventDetail.EventDetailPictureUrl = string.IsNullOrEmpty(eventDetailPictureUrl) ? eventDetail.EventDetailPictureUrl : eventDetailPictureUrl;
                eventDetail.EventId = eventDetailDto.EventId;

                _eventDetailRepo.Update(eventDetail);

                await _eventDetailRepo.SaveAsync();

                var mappedEventDetailToReturn = _mapper.Map<EventDetail, EventDetailToReturnDto>(eventDetail);

                return new ResponseHandler().EditSuccess(mappedEventDetailToReturn);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EventDetailToReturnDto>("Something went wrong");

            }
        }

        public async Task<Response<EventDetailToReturnDto>> DeleteEventDetail(int eventDetailsId)
        {
            try
            {

                var eventDetail = await _eventDetailRepo.GetByIdAsync(eventDetailsId);

                if (eventDetail is null)
                    return new ResponseHandler().NotFound<EventDetailToReturnDto>("invalid eventDetailsId");


                _eventDetailRepo.Delete(eventDetail);

                await _eventDetailRepo.SaveAsync();

                return new ResponseHandler().Deleted<EventDetailToReturnDto>();

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EventDetailToReturnDto>("Something went wrong");

            }
        }

        public async Task<Response<EventDetailToReturnDto>> GetEventDetailById(int eventDetailId)
        {
            try
            {

                var eventDetail = await _eventDetailRepo.GetByIdAsync(eventDetailId);

                if (eventDetail is null)
                    return new ResponseHandler().NotFound<EventDetailToReturnDto>("invalid eventDetailId");


                var mappedEventDetail = _mapper.Map<EventDetail, EventDetailToReturnDto>(eventDetail);

                return new ResponseHandler().Success(mappedEventDetail);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EventDetailToReturnDto>("Something went wrong");

            }
        }

        public async Task<Response<IReadOnlyList<EventDetailToReturnDto>>> GetAllEventDetail()
        {
            try
            {

                var EventDetail = await _eventDetailRepo.GetAllWithEventAsync();

                var mappedEventDetailWithEvent = _mapper.Map<IReadOnlyList<EventDetail>, IReadOnlyList<EventDetailToReturnDto>>(EventDetail);

                return new ResponseHandler().Success(mappedEventDetailWithEvent);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<IReadOnlyList<EventDetailToReturnDto>>("Something went wrong");

            }
        }

        private string ValidateOnEventDetailDto(EventDetailDto eventDetailDto)
        {
            string validationMessage = "";
            try
            {

                if (eventDetailDto.EventDetailPicture is not null)
                {
                    bool isPicture = DocumentSetting.IsImageFile(eventDetailDto.EventDetailPicture);

                    if (isPicture == false)
                        return validationMessage = "invalid picture format";
                }

                return validationMessage;
            }
            catch (Exception ex)
            {

                return validationMessage = "Something went wrong";
            }
        }

        private string ValidateOnUpdateEventDetailDto(UpdateEventDetailDto updateEventDetailDto)
        {
            string validationMessage = "";
            try
            {

                if (updateEventDetailDto.EventDetailPicture is not null)
                {
                    bool isPicture = DocumentSetting.IsImageFile(updateEventDetailDto.EventDetailPicture);

                    if (isPicture == false)
                        return validationMessage = "invalid picture format";
                }

                return validationMessage;
            }
            catch (Exception ex)
            {

                return validationMessage = "Something went wrong";
            }
        }


    }
}
