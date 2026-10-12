using AutoMapper;
using MyEntityMembershipService.DTOs.Membership.GetPackageMemberTypesDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.GetPackageMemberTypesRepository;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Membership.GetPackageMemberTypesService
{
    public class GetPackageMemberTypesService : IGetPackageMemberTypesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IGetPackageMemberTypesRepository _repo;

        public GetPackageMemberTypesService(IUnitOfWork unitOfWork, IMapper mapper, IGetPackageMemberTypesRepository repo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repo = repo;
        }
        public async Task<Response<List<GetPackageMemberTypesDto>>> GetMemberTypeNamesByIdsAsync(int packageId)
        {
            try
            {
                var memberTypesIds = await _repo.GetPackageMemberTypeIdsAsync(packageId);
                var dtos = new List<GetPackageMemberTypesDto>();
                foreach (var id in memberTypesIds)
                {
                    dtos.Add(new GetPackageMemberTypesDto() { MemberId = id });
                }
                if (memberTypesIds is null)
                    return new ResponseHandler().NotFound<List<GetPackageMemberTypesDto>>($"No members in the package with ID = {packageId}");

                var memberTypesNames = await _repo.GetMemberTypeNamesByIdsAsync(memberTypesIds);
               
                if(memberTypesNames is null)
                    return new ResponseHandler().NotFound<List<GetPackageMemberTypesDto>>($"No members in the package with ID = {packageId}");

                return new ResponseHandler().Success(memberTypesNames);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().NotFound<List<GetPackageMemberTypesDto>>(ex.InnerException?.Message ?? ex.Message);
            }
            

        }

        public async Task<Response<List<int>>> GetPackageMemberTypeIdsAsync(int packageId)
        {
            try
            {
                var memberTypesIds = await _repo.GetPackageMemberTypeIdsAsync(packageId);

                if (memberTypesIds is null)
                    return new ResponseHandler().NotFound<List<int>>($"No members in the package with ID = {packageId}");

                return new ResponseHandler().Success(memberTypesIds);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<int>>(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
