using System;
using System.Linq;
using AutoMapper;
using MyEntityMembershipService.DTOs.Membership.FieldsDto;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Membership.FieldRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FieldServ;

public class FieldService : IFieldService
{
    private readonly IFieldRepository fieldRepository;
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public FieldService(IFieldRepository fieldRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        this.fieldRepository = fieldRepository;
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }

    // ✅ GET ALL
    public async Task<Response<List<FieldDto>>> GetAllFields()
    {
        try
        {
            var fields = await fieldRepository.GetAllFields();
            var result = mapper.Map<List<FieldDto>>(fields);
            return new ResponseHandler().Success(result);
        }
        catch (Exception ex)
        {
            return new ResponseHandler().BadRequest<List<FieldDto>>(ex.Message);
        }
    }

    public async Task<Response<FieldDto>> GetFieldById(int id)
    {
        try
        {
            var field = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Field>().GetByIdAsync(id);

            if (field == null || field.IsDeleted)
                return new ResponseHandler().NotFound<FieldDto>("Field not found");

            var result = mapper.Map<FieldDto>(field);
            return new ResponseHandler().Success(result);
        }
        catch (Exception ex)
        {
            return new ResponseHandler().BadRequest<FieldDto>(ex.Message);
        }
    }

    // ✅ CREATE
    public async Task<Response<FieldDto>> CreateField(CreateFieldDto dto)
    {
        try
        {
            var normalizedName = (dto?.Title ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                return new ResponseHandler().BadRequest<FieldDto>("Field name is required.");

            var allFields = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Field>().GetAllAsync();
            var duplicateExists = allFields
                .Any(f => !f.IsDeleted && string.Equals(f.Title?.Trim(), normalizedName, StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
                return new ResponseHandler().BadRequest<FieldDto>("Field name already exists.");

            var field = mapper.Map<MyEntityMembershipService.Data.Membership.Entities.Field>(dto);

            await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Field>().Add(field);
            await unitOfWork.Save();

            var result = mapper.Map<FieldDto>(field);

            return new ResponseHandler().Success(result);
        }
        catch (Exception ex)
        {
            return new ResponseHandler().BadRequest<FieldDto>(ex.Message);
        }
    }

    // ✅ UPDATE
    public async Task<Response<FieldDto>> UpdateField(UpdateFieldDto dto)
    {
        try
        {
            var existingField = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Field>().GetByIdAsync(dto.Id);

            if (existingField == null || existingField.IsDeleted)
                return new ResponseHandler().NotFound<FieldDto>("Field not found");

            var normalizedName = (dto?.Title ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
                return new ResponseHandler().BadRequest<FieldDto>("Field name is required.");

            var allFields = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Field>().GetAllAsync();
            var duplicateExists = allFields
                .Any(f => !f.IsDeleted
                          && f.Id != dto.Id
                          && string.Equals(f.Title?.Trim(), normalizedName, StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
                return new ResponseHandler().BadRequest<FieldDto>("Field name already exists.");

            mapper.Map(dto, existingField);

            unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Field>().Update(existingField);
            await unitOfWork.Save();

            var result = mapper.Map<FieldDto>(existingField);

            return new ResponseHandler().Success(result);
        }
        catch
        {
            return new ResponseHandler().BadRequest<FieldDto>("Failed to update field.");
        }
    }

    // ✅ DELETE (Soft Delete Recommended)
    public async Task<Response<string>> DeleteField(int id)
    {
        try
        {
            var field = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Field>().GetByIdAsync(id);

            if (field == null || field.IsDeleted)
                return new ResponseHandler().NotFound<string>("Field not found");

            unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Field>().Delete(field); // or implement SoftDelete on the entity/repository
            await unitOfWork.Save();

            return new ResponseHandler().Success("Deleted successfully");
        }
        catch (Exception ex)
        {
            return new ResponseHandler().BadRequest<string>(ex.Message);
        }
    }
}