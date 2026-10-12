using AutoMapper;
using MyEntityMembershipService.APi.Repositories.Series_Repo;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.EventRepo;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Entity.EventService
{
    public class EventService : IEventService
    {
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly ISeriesRepo _seriesRepo;

        public EventService(IEventRepo eventRepo, IMapper mapper, ISeriesRepo seriesRepo)
        {
            _eventRepo = eventRepo;
            _mapper = mapper;
            _seriesRepo = seriesRepo;
        }

        public async Task<Response<EventToReturnDto>> CreateEvent(EventDto eventDto)
        {
            try
            {


                var validationResult = await ValidateOnEventDto(eventDto);

                if (validationResult.Length > 0)
                    return new ResponseHandler().BadRequest<EventToReturnDto>(validationResult);


                var mappedEvent = _mapper.Map<EventDto, Event>(eventDto);

                await _eventRepo.Add(mappedEvent);

                await _eventRepo.SaveAsync();

                var mappedEventToReturn = _mapper.Map<Event, EventToReturnDto>(mappedEvent);

                return new ResponseHandler().Created(mappedEventToReturn);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EventToReturnDto>("Something went wrong");

            }
        }


        public async Task<Response<EventToReturnDto>> UpdateEvent(UpdateEventDto eventDto, int eventId)
        {
            try
            {

                var validationResult = await ValidateOnUpdateEventDto(eventDto);

                if (validationResult.Length > 0)
                    return new ResponseHandler().BadRequest<EventToReturnDto>(validationResult);

                var evnt = await _eventRepo.GetByIdAsync(eventId);

                if (evnt is null)
                    return new ResponseHandler().NotFound<EventToReturnDto>("invalid eventId");


                //var mappedEvent = _mapper.Map<EventDto, Event>(eventDto);

                //mappedEvent.Id = evnt.Id;

                evnt.EventName = string.IsNullOrEmpty(eventDto.EventName) ? evnt.EventName : eventDto.EventName;
                evnt.Duration = string.IsNullOrEmpty(eventDto.Duration) ? evnt.Duration : eventDto.Duration;
                evnt.Location = string.IsNullOrEmpty(eventDto.Location) ? evnt.Location : eventDto.Location;
                evnt.Price = eventDto.Price.HasValue ? eventDto.Price.Value : evnt.Price.Value;
                evnt.EndDateTime = eventDto.EndDateTime.HasValue ? eventDto.EndDateTime.Value : evnt.EndDateTime;
                evnt.StartDateTime = eventDto.StartDateTime.HasValue ? eventDto.StartDateTime.Value : evnt.StartDateTime;
                evnt.SeriesId = eventDto.SeriesId;


                _eventRepo.Update(evnt);

                await _eventRepo.SaveAsync();

                var mappedEventToReturn = _mapper.Map<Event, EventToReturnDto>(evnt);

                return new ResponseHandler().EditSuccess(mappedEventToReturn);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EventToReturnDto>("Something went wrong");

            }
        }

        public async Task<Response<EventToReturnDto>> DeleteEvent(int eventId)
        {
            try
            {

                var evnt = await _eventRepo.GetByIdAsync(eventId);

                if (evnt is null)
                    return new ResponseHandler().NotFound<EventToReturnDto>("invalid eventId");


                _eventRepo.Delete(evnt);

                await _eventRepo.SaveAsync();

                return new ResponseHandler().Deleted<EventToReturnDto>();

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EventToReturnDto>("Something went wrong");

            }
        }

        public async Task<Response<EventToReturnDto>> GetEventById(int seriesId)
        {
            try
            {

                var evnt = await _eventRepo.GetByIdAsync(seriesId);

                if (evnt is null)
                    return new ResponseHandler().NotFound<EventToReturnDto>("invalid eventId");


                var eventwithEventDetail = await _eventRepo.GetByIdWithEventDetailAsync(seriesId);

                var mappedeventwithEventDetail = _mapper.Map<Event, EventToReturnDto>(eventwithEventDetail);

                return new ResponseHandler().Success(mappedeventwithEventDetail);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<EventToReturnDto>("Something went wrong");

            }
        }

        public async Task<Response<IReadOnlyList<EventToReturnDto>>> GetAllEvents()
        {
            try
            {

                var eventWithEventDetail = await _eventRepo.GetAllWithEventDetailAsync();

                var mappedeventWithEventDetail = _mapper.Map<IReadOnlyList<Event>, IReadOnlyList<EventToReturnDto>>(eventWithEventDetail);

                return new ResponseHandler().Success(mappedeventWithEventDetail);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<IReadOnlyList<EventToReturnDto>>("Something went wrong");

            }
        }


        public async Task<Response<PaginationToReturnDto<EventToReturnDto>>> GetAllEventsWithPagination(int seriesId, int pageSize = 6, int pageNumber = 1)
        {
            try
            {

                var paginationValidationResult = ValidateOnPagination(pageSize, pageNumber);

                var series = await _seriesRepo.GetByIdAsync(seriesId);

                if (series is null)
                    return new ResponseHandler().NotFound<PaginationToReturnDto<EventToReturnDto>>("Invalid seriesId");

                var countOfEvents = await _eventRepo.GetCountOfEventsAsync(seriesId);

                int totalPages = (int)Math.Ceiling(countOfEvents / (double)paginationValidationResult.pageSize);

                var eventWithEventDetail = await _eventRepo.GetAllWithEventDetailAsync(seriesId, paginationValidationResult.pageNumber, paginationValidationResult.pageSize);

                var mappedeventWithEventDetail = _mapper.Map<IReadOnlyList<Event>, IReadOnlyList<EventToReturnDto>>(eventWithEventDetail);


                var result = new PaginationToReturnDto<EventToReturnDto>(countOfEvents, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, mappedeventWithEventDetail);

                return new ResponseHandler().Success(result);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaginationToReturnDto<EventToReturnDto>>("Something went wrong");

            }
        }

        public async Task<Response<PaginationToReturnDto<EventToReturnDto>>> FilterEvents(int seriesId, FilterEventDto filters, int pageSize = 6, int pageNumber = 1)
        {
            try
            {

                var paginationValidationResult = ValidateOnPagination(pageSize, pageNumber);

                var series = await _seriesRepo.GetByIdAsync(seriesId);

                if (series is null)
                    return new ResponseHandler().NotFound<PaginationToReturnDto<EventToReturnDto>>("Invalid seriesId");

                string eventNameWithoutSpaces = string.Empty;
                string durationWithoutSpaces = string.Empty;
                string locationWithoutSpaces = string.Empty;

                if (!string.IsNullOrEmpty(filters.EventName))
                    eventNameWithoutSpaces = filters.EventName.Replace(" ", "").ToLower();

                if (!string.IsNullOrEmpty(filters.Duration))
                    durationWithoutSpaces = filters.Duration.Replace(" ", "").ToLower();


                if (!string.IsNullOrEmpty(filters.Location))
                    locationWithoutSpaces = filters.Location.Replace(" ", "").ToLower();

                Expression<Func<Event, bool>> criteria = (e) =>
                e.SeriesId == seriesId
                && (!filters.StartDateTime.HasValue || e.StartDateTime == filters.StartDateTime.Value)
                && (!filters.EndDateTime.HasValue || e.EndDateTime == filters.EndDateTime.Value)
                && (!filters.Price.HasValue || e.Price == filters.Price.Value)
                && (string.IsNullOrEmpty(filters.EventName) || e.EventName.Replace(" ", "").ToLower().Contains(eventNameWithoutSpaces))
                && (string.IsNullOrEmpty(filters.Duration) || e.Duration.Replace(" ", "").ToLower().Contains(durationWithoutSpaces))
                && (string.IsNullOrEmpty(filters.Location) || e.Location.Replace(" ", "").ToLower().Contains(locationWithoutSpaces));

                var countOfFilteredEvents = await _eventRepo.GetCountOfFilteredEventsAsync(criteria);

                int totalPages = (int)Math.Ceiling(countOfFilteredEvents / (double)paginationValidationResult.pageSize);

                var filteredEvents = await _eventRepo.FilterEventsAsync(criteria, paginationValidationResult.pageNumber, paginationValidationResult.pageSize);

                var mappedFilteredEvents = _mapper.Map<IReadOnlyList<Event>, IReadOnlyList<EventToReturnDto>>(filteredEvents);


                var result = new PaginationToReturnDto<EventToReturnDto>(countOfFilteredEvents, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, mappedFilteredEvents);

                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<PaginationToReturnDto<EventToReturnDto>>("Something went wrong");
            }
        }

        public async Task<Response<PaginationToReturnDto<EventToReturnDto>>> SortEvents(int seriesId, string sort, int pageSize, int pageNumber)
        {
            try
            {
                var series = await _seriesRepo.GetByIdAsync(seriesId);

                if (series is null)
                    return new ResponseHandler().NotFound<PaginationToReturnDto<EventToReturnDto>>("Invalid seriesId");

                var paginationValidationResult = ValidateOnPagination(pageSize, pageNumber);

                if (!string.IsNullOrEmpty(sort) && sort.Contains("Asc"))
                {
                    Expression<Func<Event, object>> criteria;

                    switch (sort)
                    {
                        case "nameAsc":
                            criteria = (e) => e.EventName;
                            break;

                        case "startDateTimeAsc":
                            criteria = (e) => e.StartDateTime;
                            break;

                        case "endDateTimeAsc":
                            criteria = (e) => e.EndDateTime;
                            break;

                        case "priceAsc":
                            criteria = (e) => e.Price;
                            break;

                        case "durationAsc":
                            criteria = (e) => e.Duration;
                            break;

                        case "locationAsc":
                            criteria = (e) => e.Location;
                            break;


                        default:
                            criteria = (e) => e.EventName;
                            break;
                    }

                    var sortedEvents = await _eventRepo.SortEventsAsync(seriesId, criteria, paginationValidationResult.pageNumber, paginationValidationResult.pageSize);

                    var mappedEvents = _mapper.Map<IReadOnlyList<Event>, IReadOnlyList<EventToReturnDto>>(sortedEvents);

                    var countOfEvents = await _eventRepo.GetCountOfEventsAsync(seriesId);

                    int totalPages = (int)Math.Ceiling(countOfEvents / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<EventToReturnDto>(countOfEvents, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, mappedEvents);

                    return new ResponseHandler().Success(result);

                }

                else if (!string.IsNullOrEmpty(sort) && sort.Contains("Desc"))
                {
                    Expression<Func<Event, object>> criteria;

                    switch (sort)
                    {
                        case "nameDesc":
                            criteria = (e) => e.EventName;
                            break;

                        case "startDateTimeDesc":
                            criteria = (e) => e.StartDateTime;
                            break;

                        case "endDateTimeDesc":
                            criteria = (e) => e.EndDateTime;
                            break;

                        case "priceDesc":
                            criteria = (e) => e.Price;
                            break;

                        case "durationDesc":
                            criteria = (e) => e.Duration;
                            break;

                        case "locationDesc":
                            criteria = (e) => e.Location;
                            break;

                        default:
                            criteria = (e) => e.EventName;
                            break;
                    }

                    var sortedEvents = await _eventRepo.SortEventsDescAsync(seriesId, criteria, paginationValidationResult.pageNumber, paginationValidationResult.pageSize);

                    var mappedEvents = _mapper.Map<IReadOnlyList<Event>, IReadOnlyList<EventToReturnDto>>(sortedEvents);

                    var countOfEvents = await _eventRepo.GetCountOfEventsAsync(seriesId);

                    int totalPages = (int)Math.Ceiling(countOfEvents / (double)paginationValidationResult.pageSize);

                    var result = new PaginationToReturnDto<EventToReturnDto>(countOfEvents, totalPages, paginationValidationResult.pageSize, paginationValidationResult.pageNumber, mappedEvents);

                    return new ResponseHandler().Success(result);
                }
                else
                {
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<EventToReturnDto>>("sorting query paramter is required");
                }
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<PaginationToReturnDto<EventToReturnDto>>("Something went wrong");
            }

        }
        private async Task<string> ValidateOnEventDto(EventDto eventDto)
        {
            string validationMessage = "";
            try
            {

                if (eventDto.Price.HasValue && eventDto.Price < 0)

                    return validationMessage = "price can't be a negative value";


                DateTime currentDateTime = DateTime.Now;

                if (eventDto.StartDateTime < currentDateTime || eventDto.EndDateTime < currentDateTime || eventDto.StartDateTime > eventDto.EndDateTime)

                    return validationMessage = "invalid start dateTime or end dateTime";

                var series = await _seriesRepo.GetByIdAsync(eventDto.SeriesId);

                if (series is null)

                    return validationMessage = "invalid seriesId";

                if (series.StartDate.HasValue || series.EndDate.HasValue)
                {

                    DateOnly datePartOfstartDateTime = DateOnly.FromDateTime(eventDto.StartDateTime);

                    DateOnly datePartOfendDateTime = DateOnly.FromDateTime(eventDto.EndDateTime);



                    if (datePartOfstartDateTime < series.StartDate || datePartOfstartDateTime > series.EndDate)
                        return validationMessage = "event start Date can't be before series start date or after series end date";

                    if (datePartOfendDateTime < series.StartDate || datePartOfendDateTime > series.EndDate)
                        return validationMessage = "event end Date can't be before series start date or after series end date";

                }
                if (series.Price.HasValue)
                {

                    if (series.Price < eventDto.Price)
                        return validationMessage = "price of event can't exceed price of series";

                    int sumOfPriceOfEvents = await _eventRepo.GetSumOfPriceOfAllEventsAsync(series.Id);

                    int newSumOfPriceOfEvents = sumOfPriceOfEvents + eventDto.Price ?? 0;

                    if (series.Price == sumOfPriceOfEvents || series.Price < newSumOfPriceOfEvents)
                        return validationMessage = "sum of price of events is equivalent to series price";
                }
                return validationMessage;
            }
            catch (Exception ex)
            {

                return validationMessage = "Something went wrong";
            }
        }

        private async Task<string> ValidateOnUpdateEventDto(UpdateEventDto eventDto)
        {
            string validationMessage = "";
            try
            {

                if (eventDto.Price.HasValue && eventDto.Price < 0)

                    return validationMessage = "price can't be a negative value";


                DateTime currentDateTime = DateTime.Now;

                if (eventDto.StartDateTime < currentDateTime || eventDto.EndDateTime < currentDateTime || eventDto.StartDateTime > eventDto.EndDateTime)

                    return validationMessage = "invalid start dateTime or end dateTime";

                var series = await _seriesRepo.GetByIdAsync(eventDto.SeriesId);

                if (series is null)

                    return validationMessage = "invalid seriesId";

                if (series.StartDate.HasValue || series.EndDate.HasValue)
                {

                    DateOnly datePartOfstartDateTime = DateOnly.FromDateTime(eventDto.StartDateTime.Value);

                    DateOnly datePartOfendDateTime = DateOnly.FromDateTime(eventDto.EndDateTime.Value);


                    if (datePartOfstartDateTime < series.StartDate || datePartOfstartDateTime > series.EndDate)
                        return validationMessage = "event start Date can't be before series start date or after series end date";

                    if (datePartOfendDateTime < series.StartDate || datePartOfendDateTime > series.EndDate)
                        return validationMessage = "event end Date can't be before series start date or after series end date";
                }

                if (series.Price.HasValue)
                {

                    if (series.Price < eventDto.Price)
                        return validationMessage = "price of event can't exceed price of series";

                    int sumOfPriceOfEvents = await _eventRepo.GetSumOfPriceOfAllEventsAsync(series.Id);

                    int newSumOfPriceOfEvents = sumOfPriceOfEvents + eventDto.Price ?? 0;

                    if (series.Price == sumOfPriceOfEvents || series.Price < newSumOfPriceOfEvents)
                        return validationMessage = "sum of price of events is equivalent to series price";

                }

                return validationMessage;
            }
            catch (Exception ex)
            {

                return validationMessage = "Something went wrong";
            }
        }

        private (int pageSize, int pageNumber) ValidateOnPagination(int pageSize, int pageNumber)
        {
            if (pageSize > 6 || pageSize < 1)
                pageSize = 6;

            if (pageNumber < 1)
                pageNumber = 1;

            return (pageSize, pageNumber);
        }
    }
}
