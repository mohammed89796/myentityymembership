using AutoMapper;
using Elastic.CommonSchema;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.NewMembershipConfigurationDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Services.Membership.NewMembershipConfigurationService
{
    public class NewMembershipConfigurationService : INewMembershipConfigurationService
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public NewMembershipConfigurationService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Response<List<NewMembershipConfigurationDto>>> GetAllConfigurations()
        {
            try
            {
                var configurations = await unitOfWork.Repository<NewMembershipConfiguration>().GetAllAsync();
                var result = mapper.Map<List<NewMembershipConfigurationDto>>(configurations);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<NewMembershipConfigurationDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<NewMembershipConfigurationDto>> GetConfigurationById(int id)
        {
            try
            {
                var configuration = await unitOfWork.Repository<NewMembershipConfiguration>().GetByIdAsync(id);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<NewMembershipConfigurationDto>("Configuration not found");
                }

                var result = mapper.Map<NewMembershipConfigurationDto>(configuration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<NewMembershipConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public async Task<Response<NewMembershipConfigurationDto>> GetConfigurationByPackageId(int packageId)
        {
            try
            {
                var configuration = await unitOfWork.Repository<NewMembershipConfiguration>()
                 .FindAsync(nc => nc.PackageId == packageId);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<NewMembershipConfigurationDto>("this Package has no New  Configuration");
                }

                var result = mapper.Map<NewMembershipConfigurationDto>(configuration);

                result.PaymentProceduresType = configuration.PaymentProceduresType;
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<NewMembershipConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public async Task<Response<NewMembershipConfigurationDto>> CreateConfiguration(NewMembershipConfigurationDto dto)
        {
            try
            {
                var entity = mapper.Map<NewMembershipConfiguration>(dto);
                await unitOfWork.Repository<NewMembershipConfiguration>().Add(entity);
                await unitOfWork.Save();

                var result = mapper.Map<NewMembershipConfigurationDto>(entity);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<NewMembershipConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<NewMembershipConfigurationDto>> UpdateConfiguration(int id, NewMembershipConfigurationDto dto)
        {
            try
            {
                var configuration = await unitOfWork.Repository<NewMembershipConfiguration>().GetByIdAsync(id);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<NewMembershipConfigurationDto>("Configuration not found");
                }

                mapper.Map(dto, configuration);
                unitOfWork.Repository<NewMembershipConfiguration>().Update(configuration);
                await unitOfWork.Save();

                var result = mapper.Map<NewMembershipConfigurationDto>(configuration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<NewMembershipConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<bool>> DeleteConfiguration(int id)
        {
            try
            {
                var configuration = await unitOfWork.Repository<NewMembershipConfiguration>().GetByIdAsync(id);
                if (configuration == null)
                {
                    return new ResponseHandler().NotFound<bool>("Configuration not found");
                }

                unitOfWork.Repository<NewMembershipConfiguration>().Delete(configuration);
                await unitOfWork.Save();

                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<NewMembershipConfigurationDto>> SyncConfiguration(int packageId, NewMembershipConfigurationDto dto)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();

                // Retrieve existing configuration
                var existingConfiguration = await unitOfWork.Repository<NewMembershipConfiguration>()
                    .FindAsync(nc => nc.PackageId == packageId);

                NewMembershipConfiguration newConfiguration = null;

                if (existingConfiguration != null)
                {
                    // Update existing configuration
                    mapper.Map(dto, existingConfiguration);
                    existingConfiguration.PackageId = packageId;

                    if(dto.PaymentProceduresType.HasValue)
                    {
                        existingConfiguration.PaymentProceduresType = dto.PaymentProceduresType.Value;
                    }

                    unitOfWork.Repository<NewMembershipConfiguration>().Update(existingConfiguration);

                    // Handle PaakageMemberForm if FormID is provided
                    if (dto.FormID != null)
                    {
                        var existingForm = await unitOfWork.Repository<PaakageMemberForm>()
                            .FindAsync(pf => pf.PackageId == packageId && pf.MemberTypeId == 5);
                        var newForm = await unitOfWork.Repository<Form>()

                       .GetByIdAsync(dto.FormID);
                        if (existingForm != null)
                        {
                            // Delete the existing entity
                            unitOfWork.Repository<PaakageMemberForm>().Delete(existingForm);

                        

                            // Add a new entity with the updated FormId
                            var newMemberForm = new PaakageMemberForm
                            {
                                PackageId = packageId,
                                MemberTypeId = 5,
                                FormId = dto.FormID,
                                Fees= newForm.Fees

                            };
                            await unitOfWork.Repository<PaakageMemberForm>().Add(newMemberForm);
                        }
                        else
                        {
                            // Add a new entity if none exists
                            var newMemberForm = new PaakageMemberForm
                            {
                                PackageId = packageId,
                                MemberTypeId = 5,
                                FormId = dto.FormID,
                                Fees = newForm.Fees

                            };
                            await unitOfWork.Repository<PaakageMemberForm>().Add(newMemberForm);
                        }
                    }
                }
                else
                {
                    // Add new configuration
                    newConfiguration = mapper.Map<NewMembershipConfiguration>(dto);
                    newConfiguration.PackageId = packageId;


                    if (!dto.PaymentProceduresType.HasValue)
                    {
                        return new ResponseHandler().BadRequest<NewMembershipConfigurationDto>("Payment Procedures Type is required.");
                    }

                    newConfiguration.PaymentProceduresType = dto.PaymentProceduresType.Value;

                    await unitOfWork.Repository<NewMembershipConfiguration>().Add(newConfiguration);

                    // Add new PaakageMemberForm if FormID is provided
                    if (dto.FormID != null)
                    {
                        var newForm = await unitOfWork.Repository<Form>()
                            .GetByIdAsync(dto.FormID);

                        var newMemberForm = new PaakageMemberForm
                        {
                            PackageId = packageId,
                            MemberTypeId = 5,
                            FormId = dto.FormID,
                            Fees=newForm.Fees
                        };
                        await unitOfWork.Repository<PaakageMemberForm>().Add(newMemberForm);
                    }
                }

                await unitOfWork.Save();
                await unitOfWork.CommitTransactionAsync();

                var result = mapper.Map<NewMembershipConfigurationDto>(existingConfiguration ?? newConfiguration);
                return new ResponseHandler().Success(result);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                return new ResponseHandler().BadRequest<NewMembershipConfigurationDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
