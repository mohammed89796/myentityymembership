using AutoMapper;
using Microsoft.IdentityModel.Tokens;

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;

using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

using MyEntityMembershipService.Repositories.Entity.FacilityTypesRepo;


namespace MyEntityMembershipService.Services.Entity.FacilityTypesService
{
    public class FacilityTypesService : IFacilityTypesService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IFacilityTypesRepository _facilityTypesRepository;

        public FacilityTypesService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IFacilityTypesRepository facilityTypesRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _facilityTypesRepository = facilityTypesRepository;
        }

        public async Task<Response<bool>> CreateFacilityType(string typeName)
        {
            try
            {
                var facilityType = new FacilityType
                {
                    name = typeName
                };

                await _unitOfWork.Repository<FacilityType>().Add(facilityType);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<FacilityType>>> GetFacilityTypesDropDown()
        {
            try
            {

                 var coreServiceList = await _facilityTypesRepository.GetAllFacilityTypesAsync();

                if (coreServiceList is null)
                {
                    return new ResponseHandler().NotFound<List<FacilityType>>("Facility types list not found");
                }

                var tasks = coreServiceList.Select(async cs =>
                {

                    var facTasks = new List<Task<string>>();
                    var newFacTasks = new List<Task<string>>();
                    
                    if (cs.Facilities != null && cs.Facilities.Any())
                    {
                        facTasks = cs.Facilities
                            .Select(async f =>
                                f.Image = await DocumentSetting.GetPreSignedUrlAsync(f.Image))
                            .ToList();
                    }

                    if (cs.NewFacilities != null && cs.NewFacilities.Any())
                    {
                        newFacTasks = cs.NewFacilities
                            .Select(async nf =>
                                nf.FacilityImageUrl = await DocumentSetting.GetPreSignedUrlAsync(nf.FacilityImageUrl))
                            .ToList();
                    }

                    await Task.WhenAll(facTasks.Concat(newFacTasks));
                });

                await Task.WhenAll(tasks);


                return new ResponseHandler().Success(coreServiceList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<FacilityType>>("Something went wrong");
            }
        }


    }
}
