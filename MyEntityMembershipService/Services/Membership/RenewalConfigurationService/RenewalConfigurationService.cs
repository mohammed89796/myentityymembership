using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.RenewalConfigurationDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.RenewalConfigurationService
{
    public class RenewalConfigurationService : IRenewalConfigurationService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RenewalConfigurationService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<List<RenewalConfigurationDto>>> GetAllConfigurations()
        {
            try
            {
                var configurations = await _unitOfWork.Repository<RenewalConfiguration>().GetAllAsync();
                var result = _mapper.Map<List<RenewalConfigurationDto>>(configurations);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<RenewalConfigurationDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<RenewalConfigurationDto>> GetConfigurationById(int id)
        {
            try
            {
                var configuration = await _unitOfWork.Repository<RenewalConfiguration>().GetByIdAsync(id);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<RenewalConfigurationDto>("Configuration not found");
                }

                var result = _mapper.Map<RenewalConfigurationDto>(configuration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<RenewalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<RenewalConfigurationDto>> GetConfigurationByPackageId(int packageId)
        {
            try
            {
                var configuration = await _unitOfWork.Repository<RenewalConfiguration>()
                                 .GetWithIncludesAsync(packageId, "PackageId",
                                     nc => nc.RenewalOptions);// Example includes                    .FindAsync(rc => rc.PackageId == packageId);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<RenewalConfigurationDto>("This package has no renewal configuration");
                }

                var result = _mapper.Map<RenewalConfigurationDto>(configuration);
                result.PaymentProceduresType = configuration.PaymentProceduresType;
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<RenewalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<RenewalConfigurationDto>> CreateConfiguration(RenewalConfigurationDto dto)
        {
            try
            {
                var entity = _mapper.Map<RenewalConfiguration>(dto);
                await _unitOfWork.Repository<RenewalConfiguration>().Add(entity);
                await _unitOfWork.Save();

                var result = _mapper.Map<RenewalConfigurationDto>(entity);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<RenewalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<RenewalConfigurationDto>> UpdateConfiguration(int id, RenewalConfigurationDto dto)
        {
            try
            {
                var configuration = await _unitOfWork.Repository<RenewalConfiguration>().GetByIdAsync(id);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<RenewalConfigurationDto>("Configuration not found");
                }

                _mapper.Map(dto, configuration);
                _unitOfWork.Repository<RenewalConfiguration>().Update(configuration);
                await _unitOfWork.Save();

                var result = _mapper.Map<RenewalConfigurationDto>(configuration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<RenewalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<bool>> DeleteConfiguration(int id)
        {
            try
            {
                var configuration = await _unitOfWork.Repository<RenewalConfiguration>().GetByIdAsync(id);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<bool>("Configuration not found");
                }

                _unitOfWork.Repository<RenewalConfiguration>().Delete(configuration);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<RenewalConfigurationDto>> SyncConfiguration(int packageId, RenewalConfigurationDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existingConfiguration = await _unitOfWork.Repository<RenewalConfiguration>()
                    .FindAsync(rc => rc.PackageId == packageId);

                RenewalConfiguration newConfiguration = null;

                if (existingConfiguration != null)
                {
                    _mapper.Map(dto, existingConfiguration);
                    existingConfiguration.PackageId = packageId;

                    if(dto.PaymentProceduresType.HasValue)
                        existingConfiguration.PaymentProceduresType = dto.PaymentProceduresType.Value;

                    _unitOfWork.Repository<RenewalConfiguration>().Update(existingConfiguration);
                }
                else
                {
                    newConfiguration = _mapper.Map<RenewalConfiguration>(dto);
                    newConfiguration.PackageId = packageId;

                    if (!dto.PaymentProceduresType.HasValue)
                    {
                        Log.Error("PaymentProceduresType is required.");

                        return new ResponseHandler()
                            .BadRequest<RenewalConfigurationDto>("Payment Procedures Type is required.");
                    }

                    newConfiguration.PaymentProceduresType = dto.PaymentProceduresType.Value;

                    await _unitOfWork.Repository<RenewalConfiguration>().Add(newConfiguration);
                }

                await _unitOfWork.Save();
                await _unitOfWork.CommitTransactionAsync();

                var result = _mapper.Map<RenewalConfigurationDto>(existingConfiguration ?? newConfiguration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResponseHandler().BadRequest<RenewalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
