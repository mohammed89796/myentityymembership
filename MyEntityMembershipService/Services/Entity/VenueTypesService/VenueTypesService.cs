using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;


using MyEntityMembershipService.Repositories.Entity.VenueTypesRepo;

namespace MyEntityMembershipService.Services.Entity.VenueTypesService
{
    public class VenueTypesService : IVenueTypesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IVenueTypesRepository _facilityTypesRepository;

        public VenueTypesService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IVenueTypesRepository facilityTypesRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _facilityTypesRepository = facilityTypesRepository;
        }

        public async Task<Response<bool>> CreateVenueType(string typeName)
        {
            try
            {
                var type = new VenueTypes
                {
                    TypeName = typeName
                };

                await _unitOfWork.Repository<VenueTypes>().Add(type);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<VenueTypes>>> GetVenueTypesDropDown()
        {
            try
            {

                 var coreServiceList = await _facilityTypesRepository.GetAllVenueTypesAsync();

                if (coreServiceList is null)
                {
                    return new ResponseHandler().NotFound<List<VenueTypes>>("Venue Type list not found");
                }

                return new ResponseHandler().Success(coreServiceList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<VenueTypes>>("Something went wrong");
            }
        }


    }
}
