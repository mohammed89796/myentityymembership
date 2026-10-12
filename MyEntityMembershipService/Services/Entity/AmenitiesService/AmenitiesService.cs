using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

using MyEntityMembershipService.Repositories.Entity.AmenitiesRepo;


namespace MyEntityMembershipService.Services.Entity.AmenitiesService
{
    public class AmenitiesService : IAmenitiesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IAmenitiesRepository _amenitiesRepository;

        public AmenitiesService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IAmenitiesRepository amenitiesRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _amenitiesRepository = amenitiesRepository;
        }

        public async Task<Response<bool>> CreateAmenity(string name)
        {
            try
            {
                var amenity = new Amenities
                {
                    name = name
                };

                await _unitOfWork.Repository<Amenities>().Add(amenity);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<Amenities>>> GetAmenitiesDropDown()
        {
            try
            {

                 var coreServiceList = await _amenitiesRepository.GetAllAmenitiesAsync();

                if (coreServiceList is null)
                {
                    return new ResponseHandler().NotFound<List<Amenities>>("Amenities list not found");
                }

                return new ResponseHandler().Success(coreServiceList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<Amenities>>("Something went wrong");
            }
        }


    }
}
