using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.RemovalConfigurationDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.RemovalConfigurationRepository;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.RemovalConfigurationService
{
    public class RemovalConfigurationService : IRemovalConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRemovalConfigurationRepository _removalRepo;

        public RemovalConfigurationService(IUnitOfWork unitOfWork, IMapper mapper, IRemovalConfigurationRepository removalRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _removalRepo = removalRepo;
        }


        public async Task<Response<RemovalConfigurationDto>> GetByPackageId(int packageId)
        {
            try
            {
                var entity = await _removalRepo.GetByPackageIdWithSubMembersAsync(packageId);

                if (entity is null)
                    return new ResponseHandler().NotFound<RemovalConfigurationDto>("RemovalConfiguration is not found.");

                var dto = _mapper.Map<RemovalConfigurationDto>(entity);
                dto.PaymentProceduresType = entity.PaymentProceduresType;
                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<RemovalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<RemovalConfigurationSyncDto>> SyncRemovalConfiguration(RemovalConfigurationSyncDto removalConfigurationDto)
        {
            try
            {
                var entity = await _removalRepo.GetByPackageIdWithSubMembersAsync(removalConfigurationDto.PackageId);

                if (entity is null)
                {

                    if (!removalConfigurationDto.PaymentProceduresType.HasValue)
                    {
                        Log.Error("PaymentProceduresType is required.");

                        return new ResponseHandler()
                            .BadRequest<RemovalConfigurationSyncDto>("Payment Procedures Type is required.");
                    }


                    entity = _mapper.Map<RemovalConfiguration>(removalConfigurationDto);
                    entity.PaymentProceduresType = removalConfigurationDto.PaymentProceduresType.Value;
                    await _removalRepo.Add(entity);
                    await _unitOfWork.Save();

                    return new ResponseHandler().Success(removalConfigurationDto, "RemovalConfiguration is added successfully.");
                }
                _mapper.Map(removalConfigurationDto, entity);

                if(removalConfigurationDto.PaymentProceduresType.HasValue)
                    entity.PaymentProceduresType = removalConfigurationDto.PaymentProceduresType.Value;

                _removalRepo.Update(entity);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(removalConfigurationDto, "RemovalConfiguration is Updated successfully.");


            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<RemovalConfigurationSyncDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<RemovalConfiguration>()
                    .FindAsync(config => config.PackageId == id);
                if (entity is null)
                    return new ResponseHandler().NotFound<bool>("RemovalConfiguration is not found.");

                _unitOfWork.Repository<RemovalConfiguration>()
                    .Delete(entity);
                await _unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<GetRemovalConfigurationDto>> GetRemovalFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null)
        {
            try
            {
                var fees = await _removalRepo.GetRemovalFeeByPackageIdAndSubMembersIdAsync(packageId, subMemberId);
                if (fees is null)
                    return new ResponseHandler().NotFound<GetRemovalConfigurationDto>("No Fees for this member.");

                var dto = new GetRemovalConfigurationDto { RemovalFees = fees };
                return new ResponseHandler().Success(dto);
            }
            catch(Exception ex)
            {
                return new ResponseHandler().BadRequest<GetRemovalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
