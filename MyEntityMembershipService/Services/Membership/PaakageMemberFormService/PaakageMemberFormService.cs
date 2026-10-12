using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.PackageRequestAuthorizationDtos;

using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.PaakageMemberFormRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Membership.PaakageMemberFormService
{
    public class PaakageMemberFormService : IPaakageMemberFormService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IPaakageMemberFormRepo paakageMemberFormRepo;

        public PaakageMemberFormService(IUnitOfWork unitOfWork, IMapper mapper, IPaakageMemberFormRepo paakageMemberFormRepo)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.paakageMemberFormRepo = paakageMemberFormRepo;

        }

        public async Task<Response<List<PaakageMemberFormDto>>> AddPaakageMemberForms(List<PaakageMemberFormDto> dtos)
        {
            try
            {
                var entities = mapper.Map<List<PaakageMemberForm>>(dtos);
                unitOfWork.Repository<PaakageMemberForm>().AddRange(entities);
                await unitOfWork.Save();

                var resultDtos = mapper.Map<List<PaakageMemberFormDto>>(entities);
                return new ResponseHandler().Success(resultDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<PaakageMemberFormDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<PaakageMemberFormDto>> UpdatePaakageMemberForm(PaakageMemberFormDto dto)
        {
            try
            {
                var entity = await unitOfWork.Repository<PaakageMemberForm>().GetByIdAsync(dto.PackageId);
                if (entity == null)
                {
                    return new ResponseHandler().NotFound<PaakageMemberFormDto>($"PaakageMemberForm with ID {dto.PackageId} not found.");
                }

                mapper.Map(dto, entity);
                unitOfWork.Repository<PaakageMemberForm>().Update(entity);
                await unitOfWork.Save();
                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaakageMemberFormDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<bool>> DeletePaakageMemberForm(int id)
        {
            try
            {
                var entity = await unitOfWork.Repository<PaakageMemberForm>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ResponseHandler().NotFound<bool>($"PaakageMemberForm with ID {id} not found.");
                }

                unitOfWork.Repository<PaakageMemberForm>().Delete(entity);
                await unitOfWork.Save();
                return new ResponseHandler().Success(true);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<bool>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<PaakageMemberFormDto>>> GetAllPaakageMemberForms()
        {
            try
            {
                var entities = await unitOfWork.Repository<PaakageMemberForm>().GetAllAsync();
                var resultDtos = mapper.Map<List<PaakageMemberFormDto>>(entities);
                return new ResponseHandler().Success(resultDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<PaakageMemberFormDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<PaakageMemberFormDto>> GetPaakageMemberFormById(int id)
        {
            try
            {
                var entity = await unitOfWork.Repository<PaakageMemberForm>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ResponseHandler().NotFound<PaakageMemberFormDto>($"PaakageMemberForm with ID {id} not found.");
                }

                var dto = mapper.Map<PaakageMemberFormDto>(entity);
                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<PaakageMemberFormDto>(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response<List<PaakageMemberFormDto>>> SyncPaakageMemberForms(int packageId, List<PaakageMemberFormDto> newForms)
        {
            try
            {
                // Fetch all forms from the repository (assuming GetAllAsync gets all records)
                var allForms =  await paakageMemberFormRepo.GetAllFormsWithoughtNew();

                // Filter forms by PackageId locally
                var existingForms = allForms.Where(f => f.PackageId == packageId).ToList();

                // 1. Find forms to delete (forms in the database but not in the new list)
                var formsToDelete = existingForms.Where(ef => !newForms
                                        .Any(nf => nf.FormId == ef.FormId && nf.MemberTypeId == ef.MemberTypeId)).ToList();

                // 2. Find forms to add (new forms not in the database)
                var formsToAdd = newForms.Where(nf => !existingForms
                                    .Any(ef => ef.FormId == nf.FormId && ef.MemberTypeId == nf.MemberTypeId)).ToList();

                // 3. Find forms to update (forms that exist in both, but might have different properties)
                var formsToUpdate = existingForms.Where(ef => newForms
                                        .Any(nf => nf.FormId == ef.FormId && nf.MemberTypeId == ef.MemberTypeId)).ToList();

                // Remove forms that are no longer in the input list (deleting one by one)
                //foreach (var formToDelete in formsToDelete)
                //{
                //  unitOfWork.Repository<PaakageMemberForm>().Delete(formToDelete);
                //}
                try
                {
                    paakageMemberFormRepo.DeleteRange(formsToDelete);

                }
                catch (Exception ex)
                {
                    return new ResponseHandler().BadRequest<List<PaakageMemberFormDto>>(ex.InnerException?.Message ?? ex.Message);
                }

                // Add new forms that were not present before
                if (formsToAdd.Any())
                {

                    foreach (var formToAdd in formsToAdd)
                    {
                        try
                        {
                            var entityToAdd = mapper.Map<PaakageMemberForm>(formToAdd);
                            paakageMemberFormRepo.Add(entityToAdd);
                        }
                        catch (Exception ex)
                        {
                            return new ResponseHandler().BadRequest<List<PaakageMemberFormDto>>(ex.InnerException?.Message ?? ex.Message);

                        }

                    }
                }

                // Update existing forms where needed
                foreach (var existingForm in formsToUpdate)
                {
                    var formDto = newForms.First(nf => nf.FormId == existingForm.FormId && nf.MemberTypeId == existingForm.MemberTypeId);

                    // Update non-key properties, e.g., Fees or other fields
                    existingForm.Fees = formDto.Fees;
                    // Map additional properties if necessary
                }

                // Save changes
                await unitOfWork.Save();

                // Return the updated forms
                var resultDtos = mapper.Map<List<PaakageMemberFormDto>>(existingForms);
                return new ResponseHandler().Success(resultDtos);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<PaakageMemberFormDto>>(ex.InnerException?.Message ?? ex.Message);
            }
        }

    }

}