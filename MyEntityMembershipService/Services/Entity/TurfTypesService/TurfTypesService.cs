using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

using MyEntityMembershipService.Repositories.Entity.TurfTypesRepo;


namespace MyEntityMembershipService.Services.Entity.TurfTypesService
{
    public class TurfTypesService : ITurfTypesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ITurfTypesRepository _facilityTypesRepository;

        public TurfTypesService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, ITurfTypesRepository facilityTypesRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _facilityTypesRepository = facilityTypesRepository;
        }

        public async Task<Response<bool>> CreateTurfType(string typeName)
        {
            try
            {
                var type = new TurfTypes
                {
                    TypeName = typeName
                };

                await _unitOfWork.Repository<TurfTypes>().Add(type);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<TurfTypes>>> GetTurfTypesDropDown()
        {
            try
            {

                 var coreServiceList = await _facilityTypesRepository.GetAllTurfTypesAsync();

                if (coreServiceList is null)
                {
                    return new ResponseHandler().NotFound<List<TurfTypes>>("Venue Type list not found");
                }

                return new ResponseHandler().Success(coreServiceList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<TurfTypes>>("Something went wrong");
            }
        }


    }
}
