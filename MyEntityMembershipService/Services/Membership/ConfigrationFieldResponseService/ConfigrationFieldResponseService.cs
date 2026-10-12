using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.ConfigurationDtos;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.ConfigurationRepo;
using MyEntityMembershipService.Repositories.Membership.MemberTypeRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FormServ;

namespace MyEntityMembershipService.Services.Membership.ConfigrationFieldResponseService
{
    public class ConfigrationFieldResponseService : IConfigrationFieldResponseService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IConfigurationRepository configurationRepository;
        private readonly IFormService formService;

        private readonly IMapper mapper;
        private readonly IMemberTypeRepo memberTypeRepo;

        public ConfigrationFieldResponseService(IUnitOfWork unitOfWork, IMapper mapper, IConfigurationRepository configurationRepository, IMemberTypeRepo memberTypeRepo, IFormService formService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.configurationRepository = configurationRepository;
            this.memberTypeRepo = memberTypeRepo;
            this.formService = formService;
        }
        public async Task<Response<List<ConfigrationFieldResponseDto>>> AddConfigrationFieldResponses(List<ConfigrationFieldResponseDto> dtos)
        {
            try
            {
                var configrationFieldResponses = mapper.Map<List<ConfigrationFieldResponse>>(dtos);
                unitOfWork.Repository<ConfigrationFieldResponse>().AddRange(configrationFieldResponses);
                await unitOfWork.Save();

                // Iterate over the saved entities
                foreach (var response in configrationFieldResponses)
                {
                    // Retrieve the Configuration based on ConfigurationId
                    var configuration = await configurationRepository
                                             .GetConfigeWithFields(response.ConfigurationId);

                    if (configuration != null && configuration.Id == 5 && response.FieldId==14)
                    {

                        //var form = await formService.GetForm(int.Parse(response.FieldResponse), response.PackageId);// Create a new record in another table if the Configuration type is "New Membership Request"
                        var NewFormPaakagemember = new PaakageMemberForm
                        {
                            PackageId = response.PackageId,
                            MemberTypeId = 5,
                            FormId = int.Parse(response.FieldResponse)
                            //Fees = form.Data.Fees
                        };
                        await unitOfWork.Repository<PaakageMemberForm>().Add(NewFormPaakagemember);
                        await unitOfWork.Save();
                    }
                }

                var resultDtos = mapper.Map<List<ConfigrationFieldResponseDto>>(configrationFieldResponses);
                return new ResponseHandler().Success(resultDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<ConfigrationFieldResponseDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<ConfigrationFieldResponseDto>> UpdateConfigrationFieldResponse(ConfigrationFieldResponseDto dto)
        {
            try
            {
                var entity = await unitOfWork.Repository<ConfigrationFieldResponse>().GetByIdAsync(dto.FieldId);
                if (entity == null)
                {
                    return new ResponseHandler().NotFound<ConfigrationFieldResponseDto>($"ConfigrationFieldResponse with ID {dto.FieldId} not found.");
                }

                mapper.Map(dto, entity);
                unitOfWork.Repository<ConfigrationFieldResponse>().Update(entity);
                await unitOfWork.Save();
                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<ConfigrationFieldResponseDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<bool>> DeleteConfigrationFieldResponse(int id)
        {
            try
            {
                var entity = await unitOfWork.Repository<ConfigrationFieldResponse>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ResponseHandler().NotFound<bool>($"ConfigrationFieldResponse with ID {id} not found.");
                }

                unitOfWork.Repository<ConfigrationFieldResponse>().Delete(entity);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<ConfigrationFieldResponseDto>>> GetAllConfigrationFieldResponses()
        {
            try
            {
                var entities = await unitOfWork.Repository<ConfigrationFieldResponse>().GetAllAsync();
                var resultDtos = mapper.Map<List<ConfigrationFieldResponseDto>>(entities);
                return new ResponseHandler().Success(resultDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<ConfigrationFieldResponseDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<ConfigrationFieldResponseDto>> GetConfigrationFieldResponseById(int id)
        {
            try
            {
                var entity = await unitOfWork.Repository<ConfigrationFieldResponse>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ResponseHandler().NotFound<ConfigrationFieldResponseDto>($"ConfigrationFieldResponse with ID {id} not found.");
                }

                var dto = mapper.Map<ConfigrationFieldResponseDto>(entity);
                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<ConfigrationFieldResponseDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<List<ConfigrationFieldResponseDto>>> SyncConfigrationFieldResponses(int packageId, List<ConfigrationFieldResponseDto> newDtos)
        {
            try
            {
                // Fetch all existing records for the given package from the repository
                var existingEntities = (await unitOfWork.Repository<ConfigrationFieldResponse>().GetAllAsync())
                    .Where(e => e.PackageId == packageId)
                    .ToList();

                // 1. Identify records to delete (those in the database but not in the new list)
                var formsToDelete = existingEntities
                    .Where(e => !newDtos.Any(n => n.ConfigurationId == e.ConfigurationId
                                                   && n.FieldId == e.FieldId)).ToList();

                // 2. Identify records to add (those in the new list but not in the database)
                var formsToAdd = newDtos
                    .Where(n => !existingEntities.Any(e => e.ConfigurationId == n.ConfigurationId
                                                            && e.FieldId == n.FieldId)).ToList();

                // 3. Identify records to update (those present in both lists but with different data)
                var formsToUpdate = existingEntities
                    .Where(e => newDtos.Any(n => n.ConfigurationId == e.ConfigurationId
                                                 && n.FieldId == e.FieldId)).ToList();

                // Remove forms that are no longer in the input list
                foreach (var formToDelete in formsToDelete)
                {
                    unitOfWork.Repository<ConfigrationFieldResponse>().Delete(formToDelete);
                }

                // Add new forms that were not present before
                if (formsToAdd.Any())
                {
                    var entitiesToAdd = mapper.Map<List<ConfigrationFieldResponse>>(formsToAdd);
                    unitOfWork.Repository<ConfigrationFieldResponse>().AddRange(entitiesToAdd);
                }

                // Update existing forms where needed
                foreach (var existingForm in formsToUpdate)
                {
                    var formDto = newDtos.First(n => n.ConfigurationId == existingForm.ConfigurationId
                                                      && n.FieldId == existingForm.FieldId);

                    // Update non-key properties
                    existingForm.FieldResponse = formDto.FieldResponse;
                    // Map additional properties if necessary
                }

                // Save changes for both the updated configuration field responses and the new PaakageMemberForm entries
                await unitOfWork.Save();
                existingEntities = (await unitOfWork.Repository<ConfigrationFieldResponse>().GetAllAsync())
                    .Where(e => e.PackageId == packageId)
                    .ToList();
                // Loop through each of the new DTOs
                // Sync the PaakageMemberForm entries for each updated ConfigrationFieldResponse
                foreach (var response in existingEntities)
                {
                    // Retrieve the Configuration based on ConfigurationId
                    var configuration = await configurationRepository.GetConfigeWithFields(response.ConfigurationId);

                    if (configuration != null && configuration.Id==5&& response.FieldId == 14)
                    {

                        // Check if an existing PaakageMemberForm record exists for this PackageId and MemberTypeId
                        var existingPackageForm = await unitOfWork.Repository<PaakageMemberForm>()
                            .FindAsync(p => p.PackageId == response.PackageId && p.MemberTypeId == 5);

                        if (existingPackageForm != null)
                        {
                            // Delete the old record
                            unitOfWork.Repository<PaakageMemberForm>().Delete(existingPackageForm);
                            await unitOfWork.Save();

                            // Add the new record with the updated FormId
                            var newForm = new PaakageMemberForm
                            {
                                PackageId = packageId,
                                MemberTypeId = 5,
                                FormId = int.Parse(response.FieldResponse)
                            };

                            unitOfWork.Repository<PaakageMemberForm>().Add(newForm);
                            await unitOfWork.Save();
                        }

                        else
                        {
                            // Create a new record in PaakageMemberForm if no existing record is found
                            var newPackageForm = new PaakageMemberForm
                            {
                                PackageId = response.PackageId,
                                MemberTypeId = 5,
                                FormId = int.Parse(response.FieldResponse)
                            };

                            // Add the new record to the PaakageMemberForm repository
                            unitOfWork.Repository<PaakageMemberForm>().Add(newPackageForm);
                        }

                        // Save the changes
                        await unitOfWork.Save();
                    }
                }

                // Return the updated forms
                var resultDtos = mapper.Map<List<ConfigrationFieldResponseDto>>(existingEntities);
                return new ResponseHandler().Success(resultDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<ConfigrationFieldResponseDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<IConfigurationResponseBase>>> GetConfigrationFieldResponse(int responseTypeID, int packageID)
        {
           try
           {
               // MemberType? memberDetails = null;

               // if (responseTypeID == 5)
               // {
               //     memberDetails = await unitOfWork.Repository<MemberType>().GetByIdAsync(responseTypeID);
               // }

               var configurationDetailsBase = await configurationRepository
                   .GetPackageDetailsByRequestTypeAndPackageID(responseTypeID, packageID);

               if (configurationDetailsBase != null && configurationDetailsBase.Any())
               {
                   List<IConfigurationResponseBase> response = new List<IConfigurationResponseBase>();

                   if (responseTypeID == 5)
                   {
                       response = configurationDetailsBase
                           .OfType<MemberConfigurationResponseDto>()
                           .Cast<IConfigurationResponseBase>()
                           .ToList();
                   }
                   else
                   {
                       response = configurationDetailsBase
                           .OfType<ConfigurationResponseDto>()
                           .Cast<IConfigurationResponseBase>()
                           .ToList();
                   }

                   return new ResponseHandler().Success(response);
               }
               else
               {
                   return new ResponseHandler().NotFound<List<IConfigurationResponseBase>>("Unable to find configuration for the given params.");
               }
           }
           catch (Exception ex)
           {
               var errorMessage = ex.InnerException?.Message ?? ex.Message;
               return new ResponseHandler().BadRequest<List<IConfigurationResponseBase>>(errorMessage);
           }
        }




    }

}
