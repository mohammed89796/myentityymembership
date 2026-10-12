using AutoMapper;
using MyEntityMembershipService.APi.Repositories.Series_Repo;

using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Entity.SeriesService
{
    public class SeriesService : ISeriesService
    {
        private readonly ISeriesRepo _seriesRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SeriesService(ISeriesRepo seriesRepo, IMapper mapper, IConfiguration configuration)
        {

            _seriesRepo = seriesRepo;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Response<SeriesToReturnDto>> CreateSeries(SeriesDto seriesDto)
        {
            try
            {

                var validationResult = ValidateOnSeriesDto(seriesDto);

                if (validationResult.Length > 0)
                    return new ResponseHandler().BadRequest<SeriesToReturnDto>(validationResult);

                var seriesPictureUrl = string.Empty;

                if (seriesDto.SeriesPicture is not null)
                {
                    var pictureUri = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", seriesDto.SeriesPicture);

                    seriesPictureUrl = pictureUri.ToString();

                }

                var mappedSeries = _mapper.Map<SeriesDto, Series>(seriesDto);

                mappedSeries.SeriesPictureUrl = seriesPictureUrl;
                await _seriesRepo.Add(mappedSeries);
                await _seriesRepo.SaveAsync();

                var mappedSeriesToReturn = _mapper.Map<Series, SeriesToReturnDto>(mappedSeries);

                return new ResponseHandler().Created(mappedSeriesToReturn);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<SeriesToReturnDto>("Something went wrong");

            }
        }


        public async Task<Response<SeriesToReturnDto>> UpdateSeries(UpdateSeriesDto seriesDto, int seriesId)
        {
            try
            {

                var validationResult = ValidateOnUpdateSeriesDto(seriesDto);

                if (validationResult.Length > 0)
                    return new ResponseHandler().BadRequest<SeriesToReturnDto>(validationResult);

                var series = await _seriesRepo.GetByIdWithEventAndEventDetailAsync(seriesId);

                if (series is null)
                    return new ResponseHandler().BadRequest<SeriesToReturnDto>("invalid seriesId");

                int sumOfPriceOfEvents = 0;

                if (seriesDto.Price.HasValue || seriesDto.StartDate.HasValue || seriesDto.EndDate.HasValue)
                {
                    if (series.Events.Count > 0)
                    {
                        foreach (var eve in series.Events)
                        {
                            if (eve.Price.HasValue)
                                sumOfPriceOfEvents += eve.Price.Value;

                            if (seriesDto.StartDate.HasValue || seriesDto.EndDate.HasValue)
                            {
                                DateOnly datePartOfstartDateTime = DateOnly.FromDateTime(eve.StartDateTime);

                                DateOnly datePartOfendDateTime = DateOnly.FromDateTime(eve.EndDateTime);


                                if (datePartOfstartDateTime < seriesDto.StartDate || datePartOfstartDateTime > seriesDto.EndDate)
                                    return new ResponseHandler().BadRequest<SeriesToReturnDto>("you have an event where it's start date is before series new start date or event start date is after series new end date");

                                if (datePartOfendDateTime < seriesDto.StartDate || datePartOfendDateTime > seriesDto.EndDate)
                                    return new ResponseHandler().BadRequest<SeriesToReturnDto>("you have an event where it's end date is before series new start date or after event end date is after series new end date");
                            }

                        }
                    }
                }

                if (seriesDto.Price.HasValue && seriesDto.Price < sumOfPriceOfEvents)
                    return new ResponseHandler().BadRequest<SeriesToReturnDto>("This series have events where sum of it's price is higher than series new price");

                var seriesPictureUrl = string.Empty;

                if (seriesDto.SeriesPicture is not null)
                {
                    var PictureUrl = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", seriesDto.SeriesPicture);

                    seriesPictureUrl = PictureUrl.ToString();
                }

                series.StartDate = seriesDto.StartDate.HasValue ? seriesDto.StartDate.Value : series.StartDate.Value;
                series.EndDate = seriesDto.EndDate.HasValue ? seriesDto.EndDate.Value : series.EndDate.Value;
                series.SeriesName = string.IsNullOrEmpty(seriesDto.SeriesName) ? series.SeriesName : seriesDto.SeriesName;
                series.SeriesPictureUrl = string.IsNullOrEmpty(seriesPictureUrl) ? series.SeriesPictureUrl : seriesPictureUrl;
                series.Description = string.IsNullOrEmpty(seriesDto.Description) ? series.Description : seriesDto.Description;
                series.AssignedAdmin = string.IsNullOrEmpty(seriesDto.AssignedAdmin) ? series.AssignedAdmin : seriesDto.AssignedAdmin;
                series.RegistrationProcess = seriesDto.RegistrationProcess.HasValue ? seriesDto.RegistrationProcess.Value : series.RegistrationProcess.Value;
                series.Price = seriesDto.Price.HasValue ? seriesDto.Price.Value : series.Price.Value;
                series.RegistrationType = seriesDto.RegistrationType.HasValue ? seriesDto.RegistrationType.Value : series.RegistrationType.Value;

                _seriesRepo.Update(series);

                await _seriesRepo.SaveAsync();

                var mappedSeriesToReturn = _mapper.Map<Series, SeriesToReturnDto>(series);

                return new ResponseHandler().EditSuccess(mappedSeriesToReturn);


            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<SeriesToReturnDto>("Something went wrong");

            }
        }

        public async Task<Response<SeriesToReturnDto>> DeleteSeries(int seriesId)
        {
            try
            {

                var series = await _seriesRepo.GetByIdAsync(seriesId);

                if (series is null)
                    return new ResponseHandler().NotFound<SeriesToReturnDto>("invalid seriesId");


                _seriesRepo.Delete(series);

                await _seriesRepo.SaveAsync();

                return new ResponseHandler().Deleted<SeriesToReturnDto>();

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<SeriesToReturnDto>("Something went wrong");

            }
        }

        public async Task<Response<SeriesToReturnDto>> GetSeriesById(int seriesId)
        {
            try
            {

                var series = await _seriesRepo.GetByIdAsync(seriesId);

                if (series is null)
                    return new ResponseHandler().NotFound<SeriesToReturnDto>("invalid seriesId");


                var serieswithEventAndEventDetail = await _seriesRepo.GetByIdWithEventAndEventDetailAsync(seriesId);

                var mappedSerieswithEventAndEventDetail = _mapper.Map<Series, SeriesToReturnDto>(serieswithEventAndEventDetail);

                return new ResponseHandler().Success(mappedSerieswithEventAndEventDetail);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<SeriesToReturnDto>("Something went wrong");

            }
        }

        public async Task<Response<object>> GetAllSeries(string? search)
        {
            try
            {
                Expression<Func<Series, bool>> criteria = null;

                if (!string.IsNullOrEmpty(search))
                {
                    string searchWithoutSpaces = search.Replace(" ", "").ToLower();

                    criteria = (s) => s.SeriesName.Replace(" ", "").ToLower().Contains(searchWithoutSpaces);
                }


                var serieswithEventAndEventDetail = await _seriesRepo.GetAllWithEventAndEventDetailAsync(criteria);

                var mappedSerieswithEventAndEventDetail = _mapper.Map<IReadOnlyList<Series>, IReadOnlyList<SeriesToReturnDto>>(serieswithEventAndEventDetail);

                bool hasData = false;

                if (mappedSerieswithEventAndEventDetail.Count > 0)
                    hasData = true;

                var result = new { mappedSerieswithEventAndEventDetail, hasData };

                return new ResponseHandler().Success<object>(result); ;

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<object>("Something went wrong");

            }
        }
        public async Task<Response<IReadOnlyList<SeriesToReturnDto>>> FilterSeries(FilterSeriesDto filters)
        {
            try
            {
                string seriesNameWithoutSpaces = string.Empty;
                string assignedAdmineWithoutSpaces = string.Empty;
                string descriptionWithoutSpaces = string.Empty;

                if (!string.IsNullOrEmpty(filters.SeriesName))
                    seriesNameWithoutSpaces = filters.SeriesName.Replace(" ", "").ToLower();

                if (!string.IsNullOrEmpty(filters.AssignedAdmin))
                    assignedAdmineWithoutSpaces = filters.AssignedAdmin.Replace(" ", "").ToLower();


                if (!string.IsNullOrEmpty(filters.Description))
                    descriptionWithoutSpaces = filters.Description.Replace(" ", "").ToLower();

                Expression<Func<Series, bool>> criteria = (s) =>
                (!filters.StartDate.HasValue || s.StartDate == filters.StartDate.Value)
                && (!filters.EndDate.HasValue || s.EndDate == filters.EndDate.Value)
                && (!filters.Price.HasValue || s.Price == filters.Price.Value)
                && (!filters.RegistrationProcess.HasValue || s.RegistrationProcess == filters.RegistrationProcess.Value)
                && (!filters.RegistrationType.HasValue || s.RegistrationType == filters.RegistrationType.Value)
                && (string.IsNullOrEmpty(filters.SeriesName) || s.SeriesName.Replace(" ", "").ToLower().Contains(seriesNameWithoutSpaces))
                && (string.IsNullOrEmpty(filters.AssignedAdmin) || s.AssignedAdmin.Replace(" ", "").ToLower().Contains(assignedAdmineWithoutSpaces))
                && (string.IsNullOrEmpty(filters.Description) || s.Description.Replace(" ", "").ToLower().Contains(descriptionWithoutSpaces));


                var filteredSeries = await _seriesRepo.FilterSeriesAsync(criteria);

                var mappedFilteredSerieswithEventAndEventDetail = _mapper.Map<IReadOnlyList<Series>, IReadOnlyList<SeriesToReturnDto>>(filteredSeries);

                return new ResponseHandler().Success(mappedFilteredSerieswithEventAndEventDetail);
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<IReadOnlyList<SeriesToReturnDto>>("Something went wrong");
            }
        }

        public async Task<Response<IReadOnlyList<SeriesToReturnDto>>> SortSeries(string sort)
        {
            try
            {

                if (!string.IsNullOrEmpty(sort) && sort.Contains("Asc"))
                {
                    Expression<Func<Series, object>> criteria;
                    switch (sort)
                    {
                        case "nameAsc":
                            criteria = (s) => s.SeriesName;
                            break;

                        case "startDateAsc":
                            criteria = (s) => s.StartDate;
                            break;

                        case "endDateAsc":
                            criteria = (s) => s.EndDate;
                            break;

                        case "priceAsc":
                            criteria = (s) => s.Price;
                            break;

                        case "registrationProcessAsc":
                            criteria = (s) => s.RegistrationProcess;
                            break;

                        case "registrationTypeAsc":
                            criteria = (s) => s.RegistrationType;
                            break;

                        case "assignedAdminAsc":
                            criteria = (s) => s.AssignedAdmin;
                            break;

                        case "descriptionAsc":
                            criteria = (s) => s.Description;
                            break;
                        default:
                            criteria = (s) => s.SeriesName;
                            break;
                    }

                    var sortedSeries = await _seriesRepo.SortSeriesAsync(criteria);

                    var mappedSortedSeriesSerieswithEventAndEventDetail = _mapper.Map<IReadOnlyList<Series>, IReadOnlyList<SeriesToReturnDto>>(sortedSeries);

                    return new ResponseHandler().Success(mappedSortedSeriesSerieswithEventAndEventDetail);
                }
                else if (!string.IsNullOrEmpty(sort) && sort.Contains("Desc"))
                {
                    Expression<Func<Series, object>> criteria;
                    switch (sort)
                    {
                        case "nameDesc":
                            criteria = (s) => s.SeriesName;
                            break;

                        case "startDateDesc":
                            criteria = (s) => s.StartDate;
                            break;

                        case "endDateDesc":
                            criteria = (s) => s.EndDate;
                            break;

                        case "priceDesc":
                            criteria = (s) => s.Price;
                            break;

                        case "registrationProcessDesc":
                            criteria = (s) => s.RegistrationProcess;
                            break;

                        case "registrationTypeDesc":
                            criteria = (s) => s.RegistrationType;
                            break;

                        case "assignedAdminDesc":
                            criteria = (s) => s.AssignedAdmin;
                            break;

                        case "descriptionDesc":
                            criteria = (s) => s.Description;
                            break;
                        default:
                            criteria = (s) => s.SeriesName;
                            break;
                    }

                    var sortedSeries = await _seriesRepo.SortSeriesDescendingAsync(criteria);

                    var mappedSortedSeriesSerieswithEventAndEventDetail = _mapper.Map<IReadOnlyList<Series>, IReadOnlyList<SeriesToReturnDto>>(sortedSeries);

                    return new ResponseHandler().Success(mappedSortedSeriesSerieswithEventAndEventDetail);
                }
                else
                {
                    return new ResponseHandler().BadRequest<IReadOnlyList<SeriesToReturnDto>>("sorting query paramter is required");
                }
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<IReadOnlyList<SeriesToReturnDto>>("Something went wrong");
            }

        }

        private string ValidateOnSeriesDto(SeriesDto seriesDto)
        {
            string validationMessage = "";
            try
            {

                if (seriesDto.SeriesPicture is not null)
                {
                    bool isPicture = DocumentSetting.IsImageFile(seriesDto.SeriesPicture);

                    if (isPicture == false)
                        return validationMessage = "invalid picture format";
                }

                if (seriesDto.Price < 0 && seriesDto.Price.HasValue)

                    return validationMessage = "price can't be a negative value";


                if (seriesDto.StartDate.HasValue && !seriesDto.EndDate.HasValue || seriesDto.EndDate.HasValue && !seriesDto.StartDate.HasValue)

                    return validationMessage = "you can't have a start date without end date or end date without start date";


                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                if (seriesDto.StartDate < currentDate || seriesDto.EndDate < currentDate || seriesDto.StartDate > seriesDto.EndDate)

                    return validationMessage = "invalid start date or end date";


                return validationMessage;
            }
            catch (Exception ex)
            {

                return validationMessage = "Something went wrong";
            }
        }

        private string ValidateOnUpdateSeriesDto(UpdateSeriesDto seriesDto)
        {
            string validationMessage = "";
            try
            {

                if (seriesDto.SeriesPicture is not null)
                {
                    bool isPicture = DocumentSetting.IsImageFile(seriesDto.SeriesPicture);

                    if (isPicture == false)
                        return validationMessage = "invalid picture format";
                }

                if (seriesDto.Price < 0 && seriesDto.Price.HasValue)

                    return validationMessage = "price can't be a negative value";


                if (seriesDto.StartDate.HasValue && !seriesDto.EndDate.HasValue || seriesDto.EndDate.HasValue && !seriesDto.StartDate.HasValue)

                    return validationMessage = "you can't have a start date without end date or end date without start date";


                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                if (seriesDto.StartDate < currentDate || seriesDto.EndDate < currentDate || seriesDto.StartDate > seriesDto.EndDate)

                    return validationMessage = "invalid start date or end date";


                return validationMessage;
            }
            catch (Exception ex)
            {

                return validationMessage = "Something went wrong";
            }
        }
    }
}
