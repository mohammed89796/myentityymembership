using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.VenueConfigurationRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Entity.VenueConfigurationService
{
    public class VenueConfigurationService : IVenueConfigurationService
    {

        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVenueConfigurationRepository _venueConfigurationRepository;
        private readonly IHttpClientFactory _httpClientFactory;



        public VenueConfigurationService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IVenueConfigurationRepository venueConfigurationRepository,
        IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {

            _unitOfWork = unitOfWork;
            this.configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _venueConfigurationRepository = venueConfigurationRepository;
            _httpClientFactory = httpClientFactory;

        }

        public async Task<Response<NewVenueBookingConfiguration>> CreateNewVenueConfigurationAsync(AddVenueConfigurationDto request)
        {
            try
            {
                if (request.VenueID <= 0 || request.PricePerSlot < 0 || request.MaxSlotsBooked <= 0 || request.SlotTime <= 0)
                {
                    return new ResponseHandler().NotFound<NewVenueBookingConfiguration>("Configuration data is invalid");
                }

                var venueExists = await _unitOfWork.Repository<NewVenue>().GetByIdAsync(request.VenueID);
                if (venueExists == null)
                {
                    return new ResponseHandler().NotFound<NewVenueBookingConfiguration>($"VenueID {request.VenueID} does not exist.");
                }

                var existingConfiguration = await _venueConfigurationRepository.GetVenueConfigurationByVenueID(request.VenueID);

                NewVenueBookingConfiguration venueConfiguration = new NewVenueBookingConfiguration();
                int savedVenueConfigurationId;

                if (existingConfiguration != null)
                {
                    var updatedConfiguration = new NewVenueBookingConfiguration
                    {
                        Id = existingConfiguration.ConfigID ?? 0,
                        VenueID = request.VenueID,
                        IsVisible = request.IsVisible,
                        AllowBooking = request.AllowBooking,
                        MaxSlotsBooked = request.MaxSlotsBooked,
                        MinSlotsBooked = request.MinSlotsBooked,
                        SlotTime = request.SlotTime,
                        CancellationTime = request.CancellationTime,
                        PricePerSlot = request.PricePerSlot,
                        BookingAvailability = request.BookingAvailability,
                        BookingAvailabilityUnit = request.BookingAvailabilityUnit
                    };

                    _unitOfWork.Repository<NewVenueBookingConfiguration>().Update(updatedConfiguration);
                    await _unitOfWork.Save(); // Save changes to ensure ID is persisted
                    savedVenueConfigurationId = updatedConfiguration.Id;
                    venueConfiguration = updatedConfiguration;
                }
                else
                {
                    venueConfiguration = new NewVenueBookingConfiguration
                    {
                        VenueID = request.VenueID,
                        IsVisible = request.IsVisible,
                        AllowBooking = request.AllowBooking,
                        MaxSlotsBooked = request.MaxSlotsBooked,
                        MinSlotsBooked = request.MinSlotsBooked,
                        SlotTime = request.SlotTime,
                        CancellationTime = request.CancellationTime,
                        PricePerSlot = request.PricePerSlot,
                        BookingAvailability = request.BookingAvailability,
                        BookingAvailabilityUnit = request.BookingAvailabilityUnit
                    };

                    await _unitOfWork.Repository<NewVenueBookingConfiguration>().Add(venueConfiguration);
                    await _unitOfWork.Save();
                    savedVenueConfigurationId = venueConfiguration.Id;
                }

                if (request.PricingRules != null && request.PricingRules.Any())
                {
                    var pricingRulesList = new List<NewVenuePricingRules>();

                    foreach (var pricingRule in request.PricingRules)
                    {
                        var pricingRuleEntity = new NewVenuePricingRules
                        {
                            Day = !string.IsNullOrEmpty(pricingRule.Day)
                                ? Enum.TryParse<DayOfWeek>(pricingRule.Day, true, out var parsedDay) ? parsedDay
                                : throw new ArgumentException($"Invalid day: {pricingRule.Day}")
                                : default,
                            StartTime = !string.IsNullOrEmpty(pricingRule.StartTime) ? TimeOnly.Parse(pricingRule.StartTime) : null,
                            EndTime = !string.IsNullOrEmpty(pricingRule.EndTime) ? TimeOnly.Parse(pricingRule.EndTime) : null,
                            CustomPrice = pricingRule.CustomPrice,
                            VenueBookingConfigurationID = savedVenueConfigurationId // Use the persisted ID
                        };

                        await _unitOfWork.Repository<NewVenuePricingRules>().Add(pricingRuleEntity);
                        pricingRulesList.Add(pricingRuleEntity);
                    }

                    await _unitOfWork.Save();

                    venueConfiguration.PricingRules = pricingRulesList;
                }

                return new ResponseHandler().Success(venueConfiguration);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<NewVenueBookingConfiguration>("Something went wrong");
            }
        }

        public async Task<Response<GetVenueConfiguration>> GetVenueConfiguration(int venueID)
        {
            try
            {
                var venueConfiguration = await _venueConfigurationRepository.GetVenueConfigurationByVenueID(venueID);

                if (venueConfiguration == null)
                {
                    return new ResponseHandler().NotFound<GetVenueConfiguration>("Venue configuration not found");
                }

                return new ResponseHandler().Success(venueConfiguration);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<GetVenueConfiguration>("Something went wrong");
            }
        }



    }
}
