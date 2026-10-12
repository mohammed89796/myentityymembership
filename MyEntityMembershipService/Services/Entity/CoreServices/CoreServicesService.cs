using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.CoreServicesRepo;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;


namespace MyEntityMembershipService.Services.Entity.CoreServices
{
    public class CoreServicesService : ICoreServicesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ICoreServicesRepository _coreServicesRepository;

        public CoreServicesService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, ICoreServicesRepository coreServicesRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _coreServicesRepository = coreServicesRepository;
        }

        public async Task<Response<bool>> CreateCoreService(string serviceName)
        {
            try
            {
                var coreService = new CoreService
                {
                    Name = serviceName
                };

                await _unitOfWork.Repository<CoreService>().Add(coreService);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<CoreService>>> GetCoreServicesDropDown()
        {
            try
            {

                 var coreServiceList = await _coreServicesRepository.GetAllCoreServicesAsync();

                if (coreServiceList is null)
                {
                    return new ResponseHandler().NotFound<List<CoreService>>("Core Services not found");
                }

                return new ResponseHandler().Success(coreServiceList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<CoreService>>("Something went wrong");
            }
        }


    }
}
