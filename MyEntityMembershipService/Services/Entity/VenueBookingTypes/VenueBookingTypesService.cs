using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;


using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

using MyEntityMembershipService.Repositories.Entity.VenueBookingTypesRepo;


namespace MyEntityMembershipService.Services.Entity.VenueBookingTypes
{
    public class VenueBookingTypesService : IVenueBookingTypesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IVenueBookingTypesRepository _venueBookingTypesRepository;



        public VenueBookingTypesService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IVenueBookingTypesRepository venueBookingTypesRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _venueBookingTypesRepository = venueBookingTypesRepository;
        }

        public async Task<Response<bool>> CreateVenueBookingTypes(string name, string description, IFormFile Icon)
        {
            try
            {

                var url = await DocumentSetting.AddToContainerAndGetUrl(_configuration["StorageConnectionString"], "buildupimagedocs", Icon);

                var bookingType = new Data.Entity.Entities.VenueBookingTypes
                {
                    IconURL = url,
                    Name = name,
                    Description = description
                };

                await _unitOfWork.Repository<Data.Entity.Entities.VenueBookingTypes>().Add(bookingType);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<Data.Entity.Entities.VenueBookingTypes>>> GetVenueBookingTypesAsync()
        {
            try
            {

                var venueBookingTypesList = await _venueBookingTypesRepository.GetAllVenueBookingTypeAsync();

                if (venueBookingTypesList is null)
                {
                    return new ResponseHandler().NotFound<List<Data.Entity.Entities.VenueBookingTypes>>("Country Codes list not found");
                }

                var tasks= venueBookingTypesList.Select(async venueBookingType =>
                {
                    if (!string.IsNullOrEmpty(venueBookingType.IconURL))
                    {
                        venueBookingType.IconURL = await DocumentSetting.GetPreSignedUrlAsync(venueBookingType.IconURL);
                    }
                });
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success(venueBookingTypesList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<Data.Entity.Entities.VenueBookingTypes>>("Something went wrong");
            }
        }


    }
}
