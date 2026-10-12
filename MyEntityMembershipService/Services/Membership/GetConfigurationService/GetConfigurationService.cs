using AutoMapper;
using MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Services.Membership.AdditionalConfigurationServ;
using MyEntityMembershipService.Services.Membership.CancellationConfigurationService;
using MyEntityMembershipService.Services.Membership.FreezeConfigurationService;
using MyEntityMembershipService.Services.Membership.NewMembershipConfigurationService;
using MyEntityMembershipService.Services.Membership.RemovalConfigurationService;
using MyEntityMembershipService.Services.Membership.RenewalConfigurationService;

namespace MyEntityMembershipService.Services.Membership.GetConfigurationService
{
    public class GetConfigurationService : IGetConfigurationService
    {
        private readonly IFreezeConfigurationService _freezeConfigService;
        private readonly IRemovalConfigurationService _removalConfigService;
        private readonly ICancellationConfigurationService _cancellationConfigService;
        private readonly IAdditionalConfigurationService _additionalConfigService;
        private readonly IRenewalConfigurationService _renewalConfigService;
        private readonly INewMembershipConfigurationService _newMembershipConfigService;
        private readonly IMapper _mapper;

        public GetConfigurationService(IFreezeConfigurationService freezeConfigService,
                                        IRemovalConfigurationService removalConfigService,
                                        ICancellationConfigurationService cancellationConfigService,
                                        IAdditionalConfigurationService additionalConfigService, 
                                        IRenewalConfigurationService renewalConfigService,
                                        INewMembershipConfigurationService newMembershipConfigService,
                                        IMapper mapper)
        {
           _freezeConfigService = freezeConfigService;
            _removalConfigService = removalConfigService;
            _cancellationConfigService = cancellationConfigService;
            _additionalConfigService = additionalConfigService;
            _renewalConfigService = renewalConfigService;
            _newMembershipConfigService = newMembershipConfigService;
            _mapper = mapper;
        }


        public async Task<Response<object>> GetConfiguartionAsync(int packageId, int requestTypesId, int? subMemberId = null)
        {
            try
            {
                if (requestTypesId == 12)
                {
                    var entity = await _freezeConfigService.GetByPackageId(packageId);
                    if (entity.Data is null)
                        return new ResponseHandler().NotFound<object>("Freeze Configuration is not found.");
                    var dto = new GetFreezeConfigurationDto 
                    
                    { PricePerAdditionalDay=entity.Data.PricePerAdditionalDay,
                    FreezeLimit=entity.Data.FreezeLimit,
                    UnitName=entity.Data.UnitName
                    };

                    return new ResponseHandler().Success<object>(dto);
                }
                if (requestTypesId == 7)
                {
                    var entity = await _renewalConfigService.GetConfigurationByPackageId(packageId);
                    if(entity.Data is null)
                        return new ResponseHandler().NotFound<object>("No Configurations was found.");

                    var dto = _mapper.Map<GetRenewalConfigurationDto>(entity.Data);
                    return new ResponseHandler().Success<object>(dto);
                }
                if (requestTypesId == 6)
                {
                    var entity = await _newMembershipConfigService.GetConfigurationByPackageId(packageId);
                    if(entity.Data is null)
                        return new ResponseHandler().NotFound<object>("No Configurations was found.");

                    var dto = _mapper.Map<GetNewMembershipConfigurationDto>(entity.Data);
                    return new ResponseHandler().Success<object>(dto);
                }
                if (requestTypesId == 16)
                {
                    var entity = await _additionalConfigService.GetAdditionFeeByPackageIdAndSubMembersIdAsync(packageId, subMemberId);
                    if(entity is null)
                        return new ResponseHandler().NotFound<object>("Additional Configuration is not found.");
                    var dto = _mapper.Map<GetAdditionalConfigurationDto>(entity);
                    return new ResponseHandler().Success<object>(dto);
                }
                if (requestTypesId == 17)
                {
                    var entity = await _cancellationConfigService.GetByPackageIdAsync(packageId);
                    if(entity.Data is null)
                        return new ResponseHandler().NotFound<object>("No Configurations was found.");

                    var dto = _mapper.Map<GetCancellationConfigurationDto>(entity.Data);
                    return new ResponseHandler().Success<object>(dto);
                }
                 if (requestTypesId == 18)
                 {
                    var removalFees = await  _removalConfigService.GetRemovalFeeByPackageIdAndSubMembersIdAsync(packageId, subMemberId);
                    if (removalFees.Data is null)
                        return new ResponseHandler().NotFound<object>("Removal Configuration is not found.");
                    return new ResponseHandler().Success<object>(removalFees.Data);
                }

                return new ResponseHandler().NotFound<object>("No Configurations was found.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<object>(ex.InnerException?.Message ?? ex.Message);
            }
            
        }
    }
}
