using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

using MyEntityMembershipService.Repositories.Entity.CoreUtilitiesRepo;


namespace MyEntityMembershipService.Services.Entity.CoreUtilities
{
    public class CoreUtilitiesService : ICoreUtilitiesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ICoreUtilitiesRepository _facilityTypesRepository;

        public CoreUtilitiesService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, ICoreUtilitiesRepository facilityTypesRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _facilityTypesRepository = facilityTypesRepository;
        }

        public async Task<Response<bool>> CreateCoreUtilitie(string typeName)
        {
            try
            {
                var coreUtility = new NewCoreUtilities
                {
                    UtilityName = typeName
                };

                await _unitOfWork.Repository<NewCoreUtilities>().Add(coreUtility);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<NewCoreUtilities>>> GetCoreUtilitiesDropDown()
        {
            try
            {

                 var coreServiceList = await _facilityTypesRepository.GetAllCoreUtilitiesAsync();

                if (coreServiceList is null)
                {
                    return new ResponseHandler().NotFound<List<NewCoreUtilities>>("Core Utilities list not found");
                }

                return new ResponseHandler().Success(coreServiceList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<NewCoreUtilities>>("Something went wrong");
            }
        }


    }
}
