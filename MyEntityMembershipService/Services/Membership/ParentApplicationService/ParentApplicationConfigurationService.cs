using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.ParentApplicationConfigurationDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Membership.ParentApplicationService
{
    /// <summary>
    ///  implementation of the IParentApplicationConfigurationService interface for managing parent application configurations.
    /// </summary>
    public class ParentApplicationConfigurationService : IParentApplicationConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParentApplicationConfigurationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<string>> CreateParentApplicationConfigAsync(CreateParentApplicationConfigDto createDto)
        {
            
            try
            {
                if (!createDto.TenantId.HasValue)
                {
                    Log.Warning("TenantId is missing in the CreateParentApplicationConfigDto for PackageId: {PackageId}", createDto.PackageId);
                    return new ResponseHandler().BadRequest<string>("TenantId is required to create a parent application configuration.");
                }


                if(createDto.PackageId <= 0)
                {
                    Log.Warning("Invalid PackageId provided in CreateParentApplicationConfigDto: {PackageId} for TenantId: {TenantId}", createDto.PackageId, createDto.TenantId);
                    return new ResponseHandler().BadRequest<string>("A valid PackageId is required to create a parent application configuration.");
                }


                var package = await _unitOfWork.Repository<Package>().FindByCriteriaAsync(p => p.Id == createDto.PackageId && p.TenantId == createDto.TenantId.Value);
                if(package is null)
                {
                    Log.Warning("Package not found for TenantId: {TenantId}, PackageId: {PackageId}", createDto.TenantId, createDto.PackageId);
                    return new ResponseHandler().NotFound<string>("Package not found for the provided TenantId and PackageId.");
                }


                var parentAppConfig = new ParentApplicationConfiguration
                {
                    PackageId = createDto.PackageId,
                    PaymentProceduresType = createDto.PaymentProceduresType
                };

                await _unitOfWork.Repository<ParentApplicationConfiguration>().Add(parentAppConfig);
                await _unitOfWork.Save();

                Log.Information("Successfully created parent application configuration with Id: {Id} for TenantId: {TenantId}, PackageId: {PackageId}", parentAppConfig.Id, createDto.TenantId, createDto.PackageId);

                return new ResponseHandler().Success<string>("Parent application configuration created successfully.");

            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error occurred while creating parent application configuration for PackageId: {PackageId}", createDto.PackageId);
                return new ResponseHandler().BadRequest<string>("An error occurred while creating the parent application configuration.");
            }
        }

        public async Task<Response<ParentApplicationConfigResponseDto>> GetParentApplicationConfigsByPackageIdAsync(int packageId)
        {
            try
            {
                Expression<Func<ParentApplicationConfiguration, bool>> filter = config => config.PackageId == packageId;

                Expression<Func<ParentApplicationConfiguration, ParentApplicationConfigResponseDto>> selector = config => new ParentApplicationConfigResponseDto
                {
                    Id = config.Id,
                    PackageId = config.PackageId,
                    PaymentProceduresType = config.PaymentProceduresType
                };


                var parentAppConfigSpec = new GeneralSpecifications<ParentApplicationConfiguration>(filter);
                var parentAppConfig = await _unitOfWork.Repository<ParentApplicationConfiguration>().GetWithSpecificationsAndProjection(parentAppConfigSpec, selector);


                if (parentAppConfig is null)
                {
                    Log.Warning("No parent application configuration found for PackageId: {PackageId}", packageId);
                    return new ResponseHandler().NotFound<ParentApplicationConfigResponseDto>("No parent application configuration found for the provided PackageId.");
                }


                Log.Information("Successfully retrieved parent application configuration for PackageId: {PackageId}", packageId);
                return new ResponseHandler().Success(parentAppConfig, "Parent application configuration retrieved successfully.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving parent application configuration for PackageId: {PackageId}", packageId);
                return new ResponseHandler().BadRequest<ParentApplicationConfigResponseDto>("An error occurred while retrieving the parent application configuration.");
            }
            
        }

        public async Task<Response<string>> UpdateParentApplicationConfigAsync(int id, UpdateParentApplicationConfigDto updateDto)
        {
            try
            {

                if(!updateDto.TenantId.HasValue)
                {
                    Log.Warning("TenantId is missing in the UpdateParentApplicationConfigDto for Id: {Id}", id);
                    return new ResponseHandler().BadRequest<string>("TenantId is required to update a parent application configuration.");
                }

                if (id <= 0)
                {
                    Log.Warning("Invalid Id provided for updating parent application configuration: {Id}", id);
                    return new ResponseHandler().BadRequest<string>("A valid Id is required to update a parent application configuration.");
                }

                var existingConfig = await _unitOfWork.Repository<ParentApplicationConfiguration>().GetByIdAsync(id);
                if (existingConfig is null)
                {
                    Log.Warning("Parent application configuration not found for Id: {Id}", id);
                    return new ResponseHandler().NotFound<string>("Parent application configuration not found for the provided Id.");
                }


                if(updateDto.PaymentProceduresType.HasValue)
                {
                    existingConfig.PaymentProceduresType = updateDto.PaymentProceduresType.Value;
                }


                if (updateDto.PackageId.HasValue)
                {
                    var package = await _unitOfWork.Repository<Package>().FindByCriteriaAsync(p => p.Id == updateDto.PackageId.Value && p.TenantId == updateDto.TenantId);
                    
                    if(package is null)
                    {
                        Log.Warning("Package not found for TenantId: {TenantId}, PackageId: {PackageId}", updateDto.TenantId, updateDto.PackageId.Value);
                        return new ResponseHandler().NotFound<string>("Package not found for the provided TenantId and PackageId.");
                    }
                    existingConfig.PackageId = updateDto.PackageId.Value;

                }

                _unitOfWork.Repository<ParentApplicationConfiguration>().Update(existingConfig);
                await _unitOfWork.Save();

                Log.Information("Successfully updated parent application configuration with Id: {Id}", id);
                return new ResponseHandler().Success<string>("Parent application configuration updated successfully.");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating parent application configuration with Id: {Id}", id);
                return new ResponseHandler().BadRequest<string>("An error occurred while updating the parent application configuration.");
            }
        }
    }
}
