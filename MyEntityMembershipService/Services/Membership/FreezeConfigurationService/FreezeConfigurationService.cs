using AutoMapper;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.FreezeConfigurtionDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.FreezeConfigurationRepository;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.FreezeConfigurationService
{
    public class FreezeConfigurationService : IFreezeConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFreezeConfigurationRepository _freezeRepo;

        public FreezeConfigurationService(IUnitOfWork unitOfWork, IMapper mapper, IFreezeConfigurationRepository freezeRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _freezeRepo = freezeRepo;
        }

        public async Task<Response<List<FreezeConfigurationDto>>> GetAllAsync()
        {
            try
            {
                var entites = await _unitOfWork.Repository<FreezeConfiguration>().GetAllAsync();
                var resultDtos = _mapper.Map<List<FreezeConfigurationDto>>(entites);
                return new ResponseHandler().Success(resultDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<FreezeConfigurationDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        /// <summary>
        /// Get Freeze Configuration by id.
        /// </summary>
        /// <param name="id">takes integer parameter</param>
        /// <returns>Response of FreezeConfigurationDto</returns>
        public async Task<Response<FreezeConfigurationDto>> GetByIdAsync(int id)
        {
            try
            {
                var freezeConfiguration = await _unitOfWork.Repository<FreezeConfiguration>().GetByIdAsync(id);
                
                if(freezeConfiguration is null)
                    return new ResponseHandler().NotFound<FreezeConfigurationDto>("FreezeConfiguration is not found.");
                
                var freezeConfigurationDto = _mapper.Map<FreezeConfigurationDto>(freezeConfiguration);
                return new ResponseHandler().Success(freezeConfigurationDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<FreezeConfigurationDto>("FreezeConfiguration is not found.");
            }
        }

        /// <summary>
        /// Get Freeze Configuration by PackageId.
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns>Response of List of FreezeConfigurationDto</returns>
        public async Task<Response<FreezeConfigurationDto>> GetByPackageId(int packageId)
        {
            try
            {
                var entities = await _freezeRepo.GetByIdWithUnitAsync(packageId);

                if (entities is null)
                    return new ResponseHandler().NotFound<FreezeConfigurationDto>("FreezeConfiguration is not found.");

                var feezeConfigurationDtos = _mapper.Map<FreezeConfigurationDto>(entities);
                feezeConfigurationDtos.PaymentProceduresType = entities.PaymentProceduresType;
                return new ResponseHandler().Success(feezeConfigurationDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<FreezeConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<FreezeConfigurationSyncDto>> SyncFreezeConfiguration(FreezeConfigurationSyncDto freezeConfigurationDto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<FreezeConfiguration>().FindAsync(config => config.PackageId == freezeConfigurationDto.PackageId);
                if (entity is null)
                {
                    if (!freezeConfigurationDto.PaymentProceduresType.HasValue)
                    {
                        Log.Error("PaymentProceduresType is required for creating a new FreezeConfiguration.");
                        return new ResponseHandler().BadRequest<FreezeConfigurationSyncDto>("PaymentProceduresType is required for creating a new FreezeConfiguration.");

                    }

                    entity = _mapper.Map<FreezeConfiguration>(freezeConfigurationDto);
                    entity.PaymentProceduresType = freezeConfigurationDto.PaymentProceduresType.Value;
                    await _unitOfWork.Repository<FreezeConfiguration>().Add(entity);
                    await _unitOfWork.Save();
                    return new ResponseHandler().Success(freezeConfigurationDto, "FreezeConfiguration created successfully.");
                }

                _mapper.Map(freezeConfigurationDto, entity);

                if (freezeConfigurationDto.PaymentProceduresType.HasValue)
                    entity.PaymentProceduresType = freezeConfigurationDto.PaymentProceduresType.Value;

                _unitOfWork.Repository<FreezeConfiguration>().Update(entity);
                await _unitOfWork.Save();
                return new ResponseHandler().Success(freezeConfigurationDto, "FreezeConfiguration updated successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<FreezeConfigurationSyncDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<FreezeConfigurationDto>> AddAsync(FreezeConfigurationDto dto)
        {
            try
            {
                if (dto is null)
                    return new ResponseHandler().BadRequest<FreezeConfigurationDto>("Can't pass empty FreezeConfiguration object!");

                var freezeConfigurationEntity = _mapper.Map<FreezeConfiguration>(dto);

                await _unitOfWork.Repository<FreezeConfiguration>().Add(freezeConfigurationEntity);
                await _unitOfWork.Save();

                var freezeConfigurationDto = _mapper.Map<FreezeConfigurationDto>(freezeConfigurationEntity);
                return new ResponseHandler().Success(freezeConfigurationDto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<FreezeConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<FreezeConfigurationDto>> UpdateAsync(FreezeConfigurationDto dto)
        {
            try
            {
                if (dto is null)
                    return new ResponseHandler().BadRequest<FreezeConfigurationDto>($"FreezeConfiguration can't be null");

                var entity = await _unitOfWork.Repository<FreezeConfiguration>().GetByIdAsync(dto.Id);

                if (entity is null)
                    return new ResponseHandler().NotFound<FreezeConfigurationDto>("FreezeConfigurtion is not found.");

                entity = _mapper.Map<FreezeConfiguration>(dto);
                _unitOfWork.Repository<FreezeConfiguration>().Update(entity);
                await _unitOfWork.Save();

                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<FreezeConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<FreezeConfiguration>().GetByIdAsync(id);

                if (entity is null)
                    return new ResponseHandler().NotFound<bool>("FreezeConfiguration is not found.");

                _unitOfWork.Repository<FreezeConfiguration>().Delete(entity);
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
