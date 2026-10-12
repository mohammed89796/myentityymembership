using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.CancellationConfigurationDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.CancellationConfigurationRepository;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.CancellationConfigurationService
{
    public class CancellationConfigurationService : ICancellationConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICancellationConfigurationRepository _cancellationRepo;

        public CancellationConfigurationService(IUnitOfWork unitOfWork, IMapper mapper, ICancellationConfigurationRepository cancellationRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cancellationRepo = cancellationRepo;
        }
        public async Task<Response<CancellationConfigurationDto>> GetByPackageIdAsync(int packageId)
        {
            try
            {
                var entity = await _cancellationRepo.GetWithUnitAsync(packageId);
                if (entity is null)
                    return new ResponseHandler().NotFound<CancellationConfigurationDto>("CancellationConfiguration is not found.");

                var dto = _mapper.Map<CancellationConfigurationDto>(entity);
                dto.PaymentProceduresType = entity.PaymentProceduresType;
                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex) 
            {
                return new ResponseHandler().BadRequest<CancellationConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<CancellationConfigurationSyncDto>> Sync(CancellationConfigurationSyncDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<CancellationConfiguration>()
                    .FindAsync(config => config.PackageId == dto.PackageId);

                if (entity is null)
                {
                    if (!dto.PaymentProceduresType.HasValue)
                    {
                        Log.Error("PaymentProceduresType is required.");

                        return new ResponseHandler()
                            .BadRequest<CancellationConfigurationSyncDto>("Payment Procedures Type is required.");
                    }


                    entity = new CancellationConfiguration
                    {
                        PackageId= dto.PackageId,
                        Refund = dto.Refund,
                        PaymentProceduresType = dto.PaymentProceduresType.Value,
                        Refund25Percent = dto.Refund25Percent,
                        Refund50Percent= dto.Refund50Percent,
                        Refund75Percent = dto.Refund75Percent,
                        Refund100Percent = dto.Refund100Percent,
                        RefundDuration= dto.RefundDuration,
                        UnitId = dto.UnitId,                        
                    };

                    await _unitOfWork.Repository<CancellationConfiguration>().Add(entity);
                    await _unitOfWork.Save();
                    return new ResponseHandler().Success(dto, "CancellationConfiguration is added successfully.");
                }

                _mapper.Map(dto, entity);

                if(dto.PaymentProceduresType.HasValue)
                    entity.PaymentProceduresType = dto.PaymentProceduresType.Value;

                _unitOfWork.Repository<CancellationConfiguration>().Update(entity);
                await _unitOfWork.Save();
                return new ResponseHandler().Success(dto, "CancellationConfiguration is added successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<CancellationConfigurationSyncDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<bool>> DeleteAsync(int packageId)
        {
            try
            {
                var entity = await _unitOfWork.Repository<CancellationConfiguration>().FindAsync(config => config.PackageId == packageId);

                if(entity is null)
                    return new ResponseHandler().NotFound<bool>("CancellationConfiguration is not found.");

                _unitOfWork.Repository<CancellationConfiguration>().Delete(entity);
                await _unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.InnerException?.Message ?? ex.Message);
            }
        }

    }
}
