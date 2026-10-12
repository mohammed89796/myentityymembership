using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.PreRegisteredUsersDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.PreRegisteredUserService
{
    public class PreRegisteredUserService : IPreRegisteredUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PreRegisteredUserService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<PreRegisteredUserToReturnDto>> GetPreRegisteredUserByFileNumberAsync(string fileNumber)
        {
            try
            {
                bool isparsed = int.TryParse(_httpContextAccessor.HttpContext.User.GetTenantId(), out int tenantId);

                if (!isparsed)
                    return new ResponseHandler().BadRequest<PreRegisteredUserToReturnDto>("Invalid tenant ID");

                var preRegisteredUser = await _unitOfWork.Repository<PreRegisteredUser>().FindAsync(pru => pru.TenantId == tenantId && fileNumber.ToLower().Replace(" ", "") == pru.FileNumber.ToLower().Replace(" ", ""));

                if (preRegisteredUser == null)
                    return new ResponseHandler().BadRequest<PreRegisteredUserToReturnDto>("Pre-registered user not found");

                var preRegisteredUserDto = _mapper.Map<PreRegisteredUser, PreRegisteredUserToReturnDto>(preRegisteredUser);

                return new ResponseHandler().Success(preRegisteredUserDto, "Pre-registered user retrieved successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting pre-registered user by file number: {FileNumber}", fileNumber);

                return new ResponseHandler().Exception<PreRegisteredUserToReturnDto>();
            }
        }
    }
}
