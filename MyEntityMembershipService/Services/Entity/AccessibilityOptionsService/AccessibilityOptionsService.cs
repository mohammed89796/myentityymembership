using AutoMapper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.AccessibilityOptionsRepos;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;


namespace MyEntityMembershipService.Services.Entity.AccessibilityOptionsService
{
    public class AccessibilityOptionsService : IAccessibilityOptionsService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IAccessibilityOptionsRepository _accessibilityOptionsRepository;

        public AccessibilityOptionsService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IAccessibilityOptionsRepository accessibilityOptionsRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _accessibilityOptionsRepository = accessibilityOptionsRepository;
        }

        public async Task<Response<bool>> CreateAccessibilityOption(string name)
        {
            try
            {
                var option = new NewAccessibilityOptions
                {
                    OptionName = name
                };

                await _unitOfWork.Repository<NewAccessibilityOptions>().Add(option);
                await _unitOfWork.Save();

                return new ResponseHandler().Created(true);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>("Something went wrong");
            }
        }
        public async Task<Response<List<NewAccessibilityOptions>>> GetAccessibilityOptionsDropDown()
        {
            try
            {

                 var coreServiceList = await _accessibilityOptionsRepository.GetAllAccessibilityOptionsAsync();

                if (coreServiceList is null)
                {
                    return new ResponseHandler().NotFound<List<NewAccessibilityOptions>>("Amenities list not found");
                }

                return new ResponseHandler().Success(coreServiceList);

            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<NewAccessibilityOptions>>("Something went wrong");
            }
        }


    }
}
