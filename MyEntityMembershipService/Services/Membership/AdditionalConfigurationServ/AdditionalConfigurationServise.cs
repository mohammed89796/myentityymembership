using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.AdditionalConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.GetConfigurationDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.AdditionalConfigurationRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using Serilog;

namespace MyEntityMembershipService.Services.Membership.AdditionalConfigurationServ
{
    public class AdditionalConfigurationService : IAdditionalConfigurationService
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IAdditionalConfigurationRepository additionalConfigurationRepository;

        public AdditionalConfigurationService(IMapper mapper, IUnitOfWork unitOfWork,IAdditionalConfigurationRepository additionalConfigurationRepository)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.additionalConfigurationRepository = additionalConfigurationRepository;
        }

        public async Task<Response<List<AdditionalConfigurationDto>>> GetAllAdditionalConfigurations()
        {
            try
            {
                var configurations = await unitOfWork.Repository<AdditionalConfiguration>().GetAllAsync();
                var result = mapper.Map<List<AdditionalConfigurationDto>>(configurations);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<AdditionalConfigurationDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<AdditionalConfigurationDto>> GetAdditionalConfigurationById(int id)
        {
            try
            {
                var configuration = await unitOfWork.Repository<AdditionalConfiguration>().GetByIdAsync(id);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<AdditionalConfigurationDto>("Configuration not found");
                }
                var result = mapper.Map<AdditionalConfigurationDto>(configuration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AdditionalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<AdditionalConfigurationDto>> GetAdditionalConfigurationByPackageId(int packageId)
        {
            try
            {
                // Use GetWithIncludesAsync to fetch the configuration with related entities
                var configuration = await unitOfWork.Repository<AdditionalConfiguration>()
                    .GetWithIncludesAsync(packageId, "PackageId",
                        nc => nc.SubMemberAdditionalConfigurations);// Example includes

                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<AdditionalConfigurationDto>("This Package has no New Configuration");
                }

                var result = mapper.Map<AdditionalConfigurationDto>(configuration);
                result.PaymentProceduresType = configuration.PaymentProceduresType; 
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AdditionalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<AdditionalConfigurationDto>> CreateAdditionalConfiguration(AdditionalConfigurationDto dto)
        {
            try
            {
                var configuration = mapper.Map<AdditionalConfiguration>(dto);
                await unitOfWork.Repository<AdditionalConfiguration>().Add(configuration);
                await unitOfWork.Save();

                var result = mapper.Map<AdditionalConfigurationDto>(configuration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AdditionalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<AdditionalConfigurationDto>> UpdateAdditionalConfiguration(int id, AdditionalConfigurationDto dto)
        {
            try
            {
                var configuration = await unitOfWork.Repository<AdditionalConfiguration>().GetByIdAsync(id);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<AdditionalConfigurationDto>("Configuration not found");
                }

                mapper.Map(dto, configuration); // Map updates from dto to entity
                unitOfWork.Repository<AdditionalConfiguration>().Update(configuration);
                await unitOfWork.Save();

                var result = mapper.Map<AdditionalConfigurationDto>(configuration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<AdditionalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<bool>> DeleteAdditionalConfiguration(int id)
        {
            try
            {
                var configuration = await unitOfWork.Repository<AdditionalConfiguration>().GetByIdAsync(id);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<bool>("Configuration not found");
                }

                 unitOfWork.Repository<AdditionalConfiguration>().Delete(configuration);
                await unitOfWork.Save();

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<AdditionalConfigurationDto>> SyncAdditionalConfiguration(int packageId, AdditionalConfigurationDto dto)
        {
            try
            {
                // Begin the transaction
                await unitOfWork.BeginTransactionAsync();

                var existingConfiguration = await additionalConfigurationRepository.GetAdditionalConfigurationWithSubMemeberByIdAsync(dto.Id.Value,dto.PackageId);
                   

                AdditionalConfiguration newConfiguration = null;

                if (existingConfiguration != null)
                {

                    if (dto.PaymentProceduresType.HasValue)
                    {
                        existingConfiguration.PaymentProceduresType = dto.PaymentProceduresType.Value;
                    }
                    
                    // Update or add SubMemberAdditionalConfiguration records
                    foreach (var subConfigDto in dto.SubMemberAdditionalConfigurations)
                    {
                        var existingSubConfig = existingConfiguration.SubMemberAdditionalConfigurations
                            .FirstOrDefault(sc => sc.SubMemberId == subConfigDto.SubMemberId);

                        if (existingSubConfig != null)
                        {
                            // Add the updated SubMemberAdditionalConfiguration
                            var unit = await unitOfWork.Repository<Unit>().FindAsyncWithNoTracking(u => u.Id == subConfigDto.UnitId);
                            if (unit != null)
                            {                                

                                existingSubConfig.Price = subConfigDto.Price;
                                existingSubConfig.Unit = unit;

                                unitOfWork.Repository<SubMemberAdditionalConfiguration>().Update(existingSubConfig);
                            }

                        }
                        else
                        {
                            var unit = await unitOfWork.Repository<Unit>().FindAsync(u => u.Id == subConfigDto.UnitId);
                            if (unit != null)
                            {
                                var newSubConfig = new SubMemberAdditionalConfiguration
                                {
                                    SubMemberId = subConfigDto.SubMemberId,
                                    Price = subConfigDto.Price,
                                    UnitId = subConfigDto.UnitId,
                                    Unit = unit, // Set the full Unit data
                                    AdditionalConfigurationId = existingConfiguration.Id // Link to the parent configuration
                                };

                                existingConfiguration.SubMemberAdditionalConfigurations.Add(newSubConfig);
                            }
                        }
                    }

                    // Update the existing configuration in the repository
                    unitOfWork.Repository<AdditionalConfiguration>().Update(existingConfiguration);
                }
                else
                {

                    if (!dto.PaymentProceduresType.HasValue)
                    {
                        Log.Error("PaymentProceduresType is required.");

                        return new ResponseHandler()
                            .BadRequest<AdditionalConfigurationDto>("Payment Procedures Type is required.");
                    }

                    // If no configuration exists, create a new one
                    newConfiguration = mapper.Map<AdditionalConfiguration>(dto);
                    newConfiguration.PackageId = packageId;
                    newConfiguration.PaymentProceduresType = dto.PaymentProceduresType.Value;
                    

                    // Initialize each SubMemberAdditionalConfiguration with full Unit data
                    foreach (var subConfigDto in dto.SubMemberAdditionalConfigurations)
                    {
                        var unit = await unitOfWork.Repository<Unit>().FindAsync(u => u.Id == subConfigDto.UnitId);
                        if (unit != null)
                        {
                            //var newSubConfig = new SubMemberAdditionalConfiguration
                            //{
                            //    SubMemberId = subConfigDto.SubMemberId,
                            //    Price = subConfigDto.Price,
                            //    UnitId = subConfigDto.UnitId,
                            //    Unit = unit, // Set the full Unit data
                            //    AdditionalConfigurationId = newConfiguration.Id // Set the foreign key for the parent configuration
                            //};

                          //  newConfiguration.SubMemberAdditionalConfigurations.Add(newSubConfig);
                        }
                    }

                    // Add the new configuration to the repository
                    await unitOfWork.Repository<AdditionalConfiguration>().Add(newConfiguration);
                }

                // Save changes after adding or updating
                await unitOfWork.Save();

                // Commit the transaction if everything is successful
                await unitOfWork.CommitTransactionAsync();

                // Map the result to DTO based on whether it's an updated or new configuration
                var result = mapper.Map<AdditionalConfigurationDto>(existingConfiguration ?? newConfiguration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                await unitOfWork.RollbackTransactionAsync();

                // Return a bad request response with the error details
                return new ResponseHandler().BadRequest<AdditionalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        //public async Task<Response<GetAdditionalConfigurationDto>> GetAdditionFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null)
        //{
        //    try
        //    {
        //        var fees = await additionalConfigurationRepository.GetAdditionFeeByPackageIdAndSubMembersIdAsync(packageId, subMemberId);
        //        if (fees is null)
        //            return new ResponseHandler().NotFound<GetAdditionalConfigurationDto>("No Fees for this member.");

        //        var dto = new GetAdditionalConfigurationDto { MembershipAdditionFormFees = fees };
        //        return new ResponseHandler().Success(dto);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseHandler().BadRequest<GetAdditionalConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
        //    }
        //}

        public async Task<GetAdditionalConfigurationDto> GetAdditionFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null)
        {
            
                var fees = await additionalConfigurationRepository.GetAdditionFeeByPackageIdAndSubMembersIdAsync(packageId, subMemberId);
               

                var dto = new GetAdditionalConfigurationDto { MembershipAdditionFormFees = fees };
                return dto;
           
        }
    }
}
