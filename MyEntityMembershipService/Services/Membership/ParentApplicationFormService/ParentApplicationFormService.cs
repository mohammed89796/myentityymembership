using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.ParentApplicationFormDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Membership.ParentApplicationFormService
{
    public class ParentApplicationFormService : IParentApplicationFormService
    {
        private const string ParentApplicationRequestTypeTitle = "ParentApplication";
        private const string MainMemberTitle = "Main Member";
        private const string ChildMemberTitle = "Child";
        private const string CoreAttributesCategoryName = "Core Attributes";

        private readonly IUnitOfWork _unitOfWork;

        public ParentApplicationFormService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<string>> CreateParentApplicationFormAsync(CreateParentApplicationFormDto dto)
        {
            try
            {
                if (!dto.TenantId.HasValue) return new ResponseHandler().BadRequest<string>("Tenant ID is required.");
                if (dto.Fees < 0) return new ResponseHandler().BadRequest<string>("Fees cannot be negative.");
                if(dto.RequestTypeId < 0) return new ResponseHandler().BadRequest<string>("Request type ID is required.");
                if (string.IsNullOrEmpty(dto.Title)) return new ResponseHandler().BadRequest<string>("Title is required.");
                if (string.IsNullOrEmpty(dto.WorkflowSelection)) return new ResponseHandler().BadRequest<string>("Workflow selection is required.");
                if (dto.MemberTypesId is null || !dto.MemberTypesId.Any()) return new ResponseHandler().BadRequest<string>("At least one member type is required.");

                // Form must belong to the "ParentApplication" request type
                var requestType = await _unitOfWork.Repository<RequestType>()
                    .FindAsync(rt => rt.Id == dto.RequestTypeId && rt.Name.Trim().ToLower() == ParentApplicationRequestTypeTitle.Trim().ToLower());
                
                if (requestType is null)
                    return new ResponseHandler().BadRequest<string>("ParentApplication request type is not configured.");

                var requestTypeFields = await _unitOfWork.Repository<RequestTypeFields>()
                .GetAllWithIncludesAsync(rtf => rtf.RequestTypeId == requestType.Id, null, null, rtf => rtf.FieldCategory);

                var coreAttributeFieldIds = requestTypeFields
                    .Where(rtf => rtf.FieldCategory != null && rtf.FieldCategory.Name.Trim().ToLower() == CoreAttributesCategoryName.Trim().ToLower())
                    .Select(rtf => rtf.FieldId)
                    .Distinct()
                    .ToList();

                if (coreAttributeFieldIds.Any())
                {
                    var selectedFieldIds = dto.ParentFieldsDto.Select(f => f.FieldId).ToHashSet();
                    var missingCoreFields = coreAttributeFieldIds.Except(selectedFieldIds).ToList();

                    if (missingCoreFields.Any())
                        return new ResponseHandler().BadRequest<string>(
                            $"All '{CoreAttributesCategoryName}' fields must be included in the form. Missing field id(s): {string.Join(", ", missingCoreFields)}.");
                }


                // Validate & split member types: Main Member -> parent form, Child -> child form
                var memberTypes = (await _unitOfWork.Repository<MemberType>()
                    .GetAllAsync(mt => dto.MemberTypesId.Contains(mt.Id))).ToList();

                if (memberTypes.Count != dto.MemberTypesId.Count)
                    return new ResponseHandler().BadRequest<string>("One or more member types are invalid.");

                var mainMemberType = memberTypes.FirstOrDefault(mt => mt.Name.Trim().ToLower() == MainMemberTitle.Trim().ToLower());
                var childMemberType = memberTypes.FirstOrDefault(mt => mt.Name.Trim().ToLower() == ChildMemberTitle.Trim().ToLower());

                var matchedCount = (mainMemberType is not null ? 1 : 0) + (childMemberType is not null ? 1 : 0);
                if (matchedCount != memberTypes.Count)
                    return new ResponseHandler().BadRequest<string>($"Member types must be either '{MainMemberTitle}' or '{ChildMemberTitle}'.");

                if (mainMemberType is null)
                    return new ResponseHandler().BadRequest<string>($"'{MainMemberTitle}' type is required.");

                if(childMemberType is null)
                    return new ResponseHandler().BadRequest<string>($"'{ChildMemberTitle}' type is required.");

                var parentForm = new Form
                {
                    Title = dto.Title,
                    Alias = dto.Alias,
                    Fees = dto.Fees,
                    WorkflowSelection = dto.WorkflowSelection,
                    RequestTypeId = requestType.Id,
                    TenantId = dto.TenantId.Value,
                    IsDraft = dto.IsDraft,
                    MemberTypes   = new List<FormMemberType> { new FormMemberType { MemberTypeId = mainMemberType.Id } },
                    FormFields = dto.ParentFieldsDto.Select(f => new FormFields
                    {
                        FieldId = f.FieldId,
                        IsRequired = f.IsRequired,
                        IsEditable = f.IsEditable,
                        Index = f.Index
                    }).ToList()
                };

                var childForm = new Form
                {
                    Title = dto.Title,
                    Alias = dto.Alias,
                    Fees = dto.Fees,
                    WorkflowSelection = dto.WorkflowSelection,
                    RequestTypeId = requestType.Id,
                    TenantId = dto.TenantId.Value,
                    IsDraft = dto.IsDraft,
                    ParentForm = parentForm, // resolves ParentFormId on save, no need for parentForm.Id upfront
                    MemberTypes = new List<FormMemberType> { new FormMemberType { MemberTypeId = childMemberType.Id } },
                    FormFields = dto.ChildFieldsDto.Select(f => new FormFields
                    {
                        FieldId = f.FieldId,
                        IsRequired = f.IsRequired,
                        IsEditable = f.IsEditable,
                        Index = f.Index
                    }).ToList()
                };

                await _unitOfWork.Repository<Form>().Add(parentForm);
                await _unitOfWork.Repository<Form>().Add(childForm);

                var customParentFieldsResult = await HandleCustomFormFields(parentForm, dto.ParentCustomFields ?? []);
                if (!customParentFieldsResult.Succeeded)
                    return customParentFieldsResult;

                var customChildFieldsResult = await HandleCustomFormFields(childForm, dto.ChildCustomFields ?? []);
                if (!customChildFieldsResult.Succeeded)
                    return customChildFieldsResult;


                await _unitOfWork.Save();

                return new ResponseHandler().Success("Parent application form created successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating the parent application form.");
                return new ResponseHandler().BadRequest<string>("An error occurred while creating the parent application form.");
            }
        }


        public async Task<Response<string>> DeleteParentApplicationFormAsync(int id, int? tenantId)
        {
            try
            {
                var parentForm = await _unitOfWork.Repository<Form>()
                    .FindAsync(f => f.Id == id && f.TenantId == tenantId && f.ParentFormId == null);

                if (parentForm is null)
                    return new ResponseHandler().BadRequest<string>("Parent application form not found.");

                var childForm = await _unitOfWork.Repository<Form>()
                    .FindAsync(f => f.ParentFormId == parentForm.Id && f.TenantId == tenantId);

                _unitOfWork.Repository<Form>().SoftDelete(parentForm);

                if (childForm is not null)
                    _unitOfWork.Repository<Form>().SoftDelete(childForm);

                await _unitOfWork.Save();

                return new ResponseHandler().Success("Parent application form deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting the parent application form.");
                return new ResponseHandler().BadRequest<string>("An error occurred while deleting the parent application form.");
            }
        }

        public async Task<Response<ParentApplicationFormDto>> GetParentApplicationFormByIdAsync(int id, int? tenantId)
        {
            try
            {
                if (!tenantId.HasValue)
                    return new ResponseHandler().BadRequest<ParentApplicationFormDto>("Tenant ID is required.");

                Expression<Func<Form, bool>> filter = f =>
                    f.Id == id && f.TenantId == tenantId && f.ParentFormId == null
                    && f.RequestType.Name.Trim().ToLower().Contains(ParentApplicationRequestTypeTitle.Trim().ToLower());

                Expression<Func<Form, ParentApplicationFormDto>> selector = f => new ParentApplicationFormDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Alias = f.Alias,
                    Fees = f.Fees,
                    WorkflowSelection = f.WorkflowSelection,
                    IsDraft = f.IsDraft,
                    UserName = f.CreatedByUsername,
                    CreationDate = f.CreationDate,
                    MainMemberTypeIds = f.MemberTypes.Select(mt => mt.MemberTypeId).ToList(),
                    ChildMemberTypeIds = f.ChildForms.SelectMany(cf => cf.MemberTypes).Select(mt => mt.MemberTypeId).ToList(),
                    ParentFieldsDto = f.FormFields.Select(ff => new FormFieldsDto
                    {
                        FieldId = ff.FieldId,
                        Index = ff.Index,
                        IsEditable = ff.IsEditable,
                        IsRequired = ff.IsRequired
                    }).ToList(),
                    ChildFieldsDto = f.ChildForms.SelectMany(cf => cf.FormFields).Select(ff => new FormFieldsDto
                    {
                        FieldId = ff.FieldId,
                        Index = ff.Index,
                        IsEditable = ff.IsEditable,
                        IsRequired = ff.IsRequired
                    }).ToList()
                };

                var spec = new GeneralSpecifications<Form>(filter);
                spec.AddInclude(q => q.Include(f => f.RequestType));

                var dto = await _unitOfWork.Repository<Form>()
                    .GetWithSpecificationsAndProjection(spec, selector);

                if (dto is null)
                    return new ResponseHandler().BadRequest<ParentApplicationFormDto>("Parent application form not found.");

                return new ResponseHandler().Success(dto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving the parent application form by ID.");
                return new ResponseHandler().BadRequest<ParentApplicationFormDto>("An error occurred while retrieving the parent application form by ID.");
            }
        }

        public async Task<Response<PaginationToReturnDto<ParentApplicationFormDto>>> GetParentApplicationFormsAsync(ParentApplicationFormFilterDto filterDto)
        {
            try
            {

                if(!filterDto.TenantId.HasValue)
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<ParentApplicationFormDto>>("Tenant ID is required.");

                Expression<Func<Form, bool>> filter = f =>
                    f.ParentFormId == null &&
                    f.RequestType.Name.Trim().ToLower().Contains(ParentApplicationRequestTypeTitle.Trim().ToLower()) &&
                    f.TenantId == filterDto.TenantId.Value &&
                    (string.IsNullOrEmpty(filterDto.SearchTerm) || f.Title.Trim().ToLower().Contains(filterDto.SearchTerm.Trim().ToLower()));

                Expression<Func<Form, ParentApplicationFormDto>> selector = f => new ParentApplicationFormDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Alias = f.Alias,
                    Fees = f.Fees,
                    WorkflowSelection = f.WorkflowSelection,
                    IsDraft = f.IsDraft,
                    UserName = f.CreatedByUsername,
                    CreationDate = f.CreationDate,
                    MainMemberTypeIds = f.MemberTypes.Select(mt => mt.MemberTypeId).ToList(),
                    ChildMemberTypeIds = f.ChildForms.SelectMany(cf => cf.MemberTypes).Select(mt => mt.MemberTypeId).ToList(),
                    ParentFieldsDto = f.FormFields.Select(ff => new FormFieldsDto
                    {
                        FieldId = ff.FieldId,
                        Index = ff.Index,
                        IsEditable = ff.IsEditable,
                        IsRequired = ff.IsRequired
                    }).ToList(),
                    ChildFieldsDto = f.ChildForms.SelectMany(cf => cf.FormFields).Select(ff => new FormFieldsDto
                    {
                        FieldId = ff.FieldId,
                        Index = ff.Index,
                        IsEditable = ff.IsEditable,
                        IsRequired = ff.IsRequired
                    }).ToList()
                };

                var count = await _unitOfWork.Repository<Form>().GetCountByCriteriaAsync(filter);

                var spec = new GeneralSpecifications<Form>(filter, filterDto.PageNumber, filterDto.PageSize);
                spec.SetOrderByDescending(f => f.Id);
                spec.AddInclude(q => q.Include(f => f.RequestType));

                var dtos = await _unitOfWork.Repository<Form>()
                    .GetAllWithSpecificationsAndProjection(spec, selector);

                var totalPages = (int)Math.Ceiling(count / (double)filterDto.PageSize);

                return new ResponseHandler().Success(new PaginationToReturnDto<ParentApplicationFormDto>(
                    count,
                    totalPages,
                    filterDto.PageSize,
                    filterDto.PageNumber,
                    dtos));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while retrieving parent application forms.");
                return new ResponseHandler().BadRequest<PaginationToReturnDto<ParentApplicationFormDto>>("An error occurred while retrieving parent application forms.");
            }
        }

        public async Task<Response<string>> UpdateParentApplicationFormAsync(UpdateParentApplicationFormDto dto)
        {
            try
            {
                if (!dto.TenantId.HasValue)
                    return new ResponseHandler().BadRequest<string>("Tenant ID is required.");

                var parentForm = (await _unitOfWork.Repository<Form>().GetAllWithIncludesAsync(
                    f => f.Id == dto.Id && f.TenantId == dto.TenantId && f.ParentFormId == null,
                    null, null,
                    f => f.FormFields,
                    f => f.MemberTypes)).FirstOrDefault();

                if (parentForm is null)
                    return new ResponseHandler().BadRequest<string>("Parent application form not found.");

                var childForm = (await _unitOfWork.Repository<Form>().GetAllWithIncludesAsync(
                    f => f.ParentFormId == parentForm.Id && f.TenantId == dto.TenantId,
                    null, null,
                    f => f.FormFields,
                    f => f.MemberTypes)).FirstOrDefault();

                if (childForm is null)
                    return new ResponseHandler().BadRequest<string>("Child application form not found.");

                // Shared scalar fields (mirrors how CreateParentApplicationFormAsync duplicates them across both forms)
                if (dto.Title is not null)
                {
                    parentForm.Title = dto.Title;
                    childForm.Title = dto.Title;
                }

                if (dto.Alias is not null)
                {
                    parentForm.Alias = dto.Alias;
                    childForm.Alias = dto.Alias;
                }

                if (dto.Fees.HasValue)
                {
                    if (dto.Fees.Value < 0)
                        return new ResponseHandler().BadRequest<string>("Fees cannot be negative.");

                    parentForm.Fees = dto.Fees.Value;
                    childForm.Fees = dto.Fees.Value;
                }

                if (dto.WorkflowSelection is not null)
                {
                    parentForm.WorkflowSelection = dto.WorkflowSelection;
                    childForm.WorkflowSelection = dto.WorkflowSelection;
                }

                if (dto.IsDraft.HasValue)
                {
                    parentForm.IsDraft = dto.IsDraft.Value;
                    childForm.IsDraft = dto.IsDraft.Value;
                }

                // Member types: re-validate and split into Main Member / Child, same as create
                if (dto.MemberTypesId is not null)
                {
                    if (!dto.MemberTypesId.Any())
                        return new ResponseHandler().BadRequest<string>("At least one member type is required.");

                    var memberTypes = (await _unitOfWork.Repository<MemberType>()
                        .GetAllAsync(mt => dto.MemberTypesId.Contains(mt.Id))).ToList();

                    if (memberTypes.Count != dto.MemberTypesId.Count)
                        return new ResponseHandler().BadRequest<string>("One or more member types are invalid.");

                    var mainMemberType = memberTypes.FirstOrDefault(mt => mt.Name.Trim().ToLower() == MainMemberTitle.Trim().ToLower());
                    var childMemberType = memberTypes.FirstOrDefault(mt => mt.Name.Trim().ToLower() == ChildMemberTitle.Trim().ToLower());

                    var matchedCount = (mainMemberType is not null ? 1 : 0) + (childMemberType is not null ? 1 : 0);
                    if (matchedCount != memberTypes.Count)
                        return new ResponseHandler().BadRequest<string>($"Member types must be either '{MainMemberTitle}' or '{ChildMemberTitle}'.");

                    if (mainMemberType is null)
                        return new ResponseHandler().BadRequest<string>($"'{MainMemberTitle}' type is required.");

                    if (childMemberType is null)
                        return new ResponseHandler().BadRequest<string>($"'{ChildMemberTitle}' type is required.");

                    parentForm.MemberTypes.Clear();
                    parentForm.MemberTypes.Add(new FormMemberType { MemberTypeId = mainMemberType.Id });

                    childForm.MemberTypes.Clear();
                    childForm.MemberTypes.Add(new FormMemberType { MemberTypeId = childMemberType.Id });
                }

                // Field sets: full replace when provided
                if (dto.ParentFieldsDto is not null)
                {
                    parentForm.FormFields.Clear();
                    foreach (var f in dto.ParentFieldsDto)
                    {
                        parentForm.FormFields.Add(new FormFields
                        {
                            FieldId = f.FieldId,
                            IsRequired = f.IsRequired,
                            IsEditable = f.IsEditable,
                            Index = f.Index
                        });
                    }
                }

                if (dto.ChildFieldsDto is not null)
                {
                    childForm.FormFields.Clear();
                    foreach (var f in dto.ChildFieldsDto)
                    {
                        childForm.FormFields.Add(new FormFields
                        {
                            FieldId = f.FieldId,
                            IsRequired = f.IsRequired,
                            IsEditable = f.IsEditable,
                            Index = f.Index
                        });
                    }
                }

                // New custom fields are appended; existing custom Field records are not diffed/updated here
                if (dto.ParentCustomFields is not null && dto.ParentCustomFields.Any())
                {
                    var startIndex = parentForm.FormFields?.Count ?? 0;

                    var customFields = dto.ParentCustomFields.Select((f, i) => new Field
                    {
                        Title = f.Title,
                        TitleAr = f.TitleAr,
                        FieldTypeId = f.FieldTypeId,
                        UnitTypeId = f.UnitTypeId,
                        MapField = f.MapField,
                        DropdownValues = f.DropdownValues?.Select(v => new DropdownValue { Value = v }).ToList(),
                        FormFields = new List<FormFields>
                {
                    new FormFields
                    {
                        Form = parentForm,
                        IsRequired = f.IsRequired,
                        IsHidden = f.IsAdminFilled,
                        Index = startIndex + i
                    }
                }
                    }).ToList();

                    _unitOfWork.Repository<Field>().AddRange(customFields);
                }

                _unitOfWork.Repository<Form>().Update(parentForm);
                _unitOfWork.Repository<Form>().Update(childForm);

                await _unitOfWork.Save();

                return new ResponseHandler().Success("Parent application form updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating the parent application form.");
                return new ResponseHandler().BadRequest<string>("An error occurred while updating the parent application form.");
            }
        }

        private async Task<Response<string>> HandleCustomFormFields(Form form, List<CustomParentApplicationFormFieldsDto> dto)
        {
            try
            {
                if (dto is null || !dto.Any())
                {
                    Log.Information("No custom form fields to handle.");
                    return new ResponseHandler().Success("No custom form fields to handle.");
                }

                var startIndex = form.FormFields?.Count ?? 0;

                var customFields = dto.Select((f, i) => new Field
                {
                    Title = f.Title,
                    TitleAr = f.TitleAr,
                    FieldTypeId = f.FieldTypeId,
                    UnitTypeId = f.UnitTypeId,
                    MapField = f.MapField,
                    DropdownValues = f.DropdownValues?.Select(v => new DropdownValue { Value = v }).ToList(),
                    FormFields = new List<FormFields>
                    {
                        new FormFields
                        {
                            Form = form,
                            IsRequired = f.IsRequired,
                            IsHidden = f.IsAdminFilled,
                            Index = startIndex + i
                        }
                    }
                }).ToList();

                Log.Information($"Mapped {customFields.Count} custom fields from DTO.");

                _unitOfWork.Repository<Field>().AddRange(customFields);

                Log.Information($"Linked {customFields.Count} custom fields to Form ID {form.Id}.");

                return new ResponseHandler().Success("Custom fields created and linked successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while handling custom form fields.");
                return new ResponseHandler().BadRequest<string>("Something went wrong!");
            }
        }
    }
}