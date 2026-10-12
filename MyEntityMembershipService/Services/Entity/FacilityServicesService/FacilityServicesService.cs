using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

using MyEntityMembershipService.Repositories.Entity.FacilityServicesRepo;


namespace MyEntityMembershipService.Services.Entity.FacilityServicesService
{
    public class FacilityServicesService : IFacilityServicesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IFacilityServicesRepository _facilityServicesRepository;

        public FacilityServicesService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IFacilityServicesRepository facilityServicesRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _facilityServicesRepository = facilityServicesRepository;
        }

        public async Task<Response<bool>> CreateFacilityService(string serviceName)
        {
            try
            {
                var facilityService = new CoreFacilityServices
                {
                    Name = serviceName
                };

                await _unitOfWork.Repository<CoreFacilityServices>().Add(facilityService);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<CoreFacilityServices>>> GetFacilityServicesDropDown()
        {
            try
            {

                 var coreServiceList = await _facilityServicesRepository.GetAllFacilityServicesAsync();

                if (coreServiceList is null)
                {
                    return new ResponseHandler().NotFound<List<CoreFacilityServices>>("Facility Services list not found");
                }

                return new ResponseHandler().Success(coreServiceList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<CoreFacilityServices>>("Something went wrong");
            }
        }


    }
}
