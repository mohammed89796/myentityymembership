using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.AttainmentDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;

namespace MyEntityMembershipService.Services.Entity.AttainmentService
{
    public class AttainmentService : IAttainmentService
    {
        private readonly IUnitOfWork unitOfWork;

        public AttainmentService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public async Task<Response<bool>> CreateAttainmentAsync(AddAttainmentDto dto, string tenantId)
        {
            try
            {
                bool isParsed = int.TryParse(tenantId, out int tenantIdInt);
                if (!isParsed)
                    return new ResponseHandler().BadRequest<bool>("Invalid Tenant Id");

                var validationMessage = await ValidateOnCreationOfAttainment(dto);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    return new ResponseHandler().BadRequest<bool>(validationMessage);
                }
                var attainmentCategory = await unitOfWork.Repository<AttainmentCategory>()
               .GetByIdAsync(dto.AttainmentCategoryId);

                if (attainmentCategory is null)
                {
                    return new ResponseHandler().BadRequest<bool>("Invalid Attainment Category Id");
                }

                var attainment = new Attainment
                {
                    Title = dto.Title,
                    IssuingOrganization = dto.IssuingOrganization,
                    IssuingDate = dto.IssuingDate,
                    ExpiryDate = dto.ExpiryDate,
                    AttainmentCategoryId = dto.AttainmentCategoryId,
                    TenantId = tenantIdInt

                };



                string attachedDocumentPath = string.Empty;

                if (dto.AttachedDocument != null)
                {
                    attachedDocumentPath = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", dto.AttachedDocument);
                }
                attainment.AttachedDocument = attachedDocumentPath;

                await unitOfWork.Repository<Attainment>().Add(attainment);
                await unitOfWork.Save();

                Log.Information("Attainment created successfully with Id: {AttainmentId}", attainment.Id);

                return new ResponseHandler().Created(true, "Attainment created successfully.");
            }
            catch (Exception ex)
            {

                Log.Error("An error occurred while creating attainment: {ErrorMessage}", ex.Message);

                return new ResponseHandler().Exception<bool>("An error occurred while creating attainment.");
            }
        }

        public async Task<Response<bool>> DeleteAttainment(int attainmentId)
        {
            try
            {
                var attainment = await unitOfWork.Repository<Attainment>()
                    .GetByIdAsync(attainmentId);

                if (attainment == null)
                {
                    return new ResponseHandler().NotFound<bool>("Attainment not found.");
                }
                unitOfWork.Repository<Attainment>().SoftDelete(attainment);

                await unitOfWork.Save();

                Log.Information("Attainment with Id: {AttainmentId} deleted successfully.", attainmentId);

                return new ResponseHandler().Success(true, "Attainment deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while deleting attainment with Id: {AttainmentId}. Error: {ErrorMessage}", attainmentId, ex.Message);

                return new ResponseHandler().Exception<bool>("An error occurred while deleting attainment.");
            }
        }

        public async Task<Response<bool>> UpdateAttainmentAsync(UpdateAttainmentDto dto)
        {
            try
            {
                var validationMessage = await ValidateOnAttainmentUpdate(dto);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    return new ResponseHandler().BadRequest<bool>(validationMessage);
                }

                var attainment = await unitOfWork.Repository<Attainment>().GetByIdAsync(dto.Id);

                if (attainment is null)
                    return new ResponseHandler().BadRequest<bool>("Invalid Attainment Id");


                string attachedDocumentPath = string.Empty;

                if (dto.AttachedDocument != null)
                {
                    attachedDocumentPath = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", dto.AttachedDocument);

                    attainment.AttachedDocument = attachedDocumentPath;
                }

                if (dto.AttachedDocumentRemoved)
                    attainment.AttachedDocument = string.Empty;

                attainment.Title = dto.Title;

                attainment.IssuingOrganization = dto.IssuingOrganization;

                attainment.IssuingDate = dto.IssuingDate;

                attainment.ExpiryDate = dto.ExpiryDate;

                attainment.AttainmentCategoryId = dto.AttainmentCategoryId;

                unitOfWork.Repository<Attainment>().Update(attainment);

                await unitOfWork.Save();

                return new ResponseHandler().EditSuccess(true, "Attainment updated successfully.");

            }
            catch (Exception ex)
            {

                Log.Error("An error occurred while updating attainment with Id: {AttainmentId}. Error: {ErrorMessage}", dto.Id, ex.Message);

                return new ResponseHandler().Exception<bool>($"An error occurred while updating attainment with Id: {dto.Id}");

            }
        }
public async Task<Response<AttainmentToReturnDto>> GetAttainmentByIdAsync(int attainmentId)
        {
            try
            {
                var spec = new GeneralSpecifications<Attainment>(a => a.Id == attainmentId);
 
                spec.AddInclude(iq => iq.Include(a => a.AttainmentCategory));
 
                spec.AsNoTracking = true;
 
                var attainment = await unitOfWork.Repository<Attainment>().GetWithSpecifications(spec);
 
 
                if (attainment == null)
                {
                    return new ResponseHandler().NotFound<AttainmentToReturnDto>("Attainment not found.");
                }
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
 
                var attainmentDto = new AttainmentToReturnDto
                {
                    Id = attainment.Id,
                    Title = attainment.Title,
                    AttainmentCategoryId = attainment.AttainmentCategoryId,
                    AttainmentCategoryName = attainment.AttainmentCategory.Name,
                    AttainmentCategoryArabicName = attainment.AttainmentCategory.ArabicName,
                    IssuingOrganization = attainment.IssuingOrganization,
                    IssuingDate = attainment.IssuingDate,
                    ExpiryDate = attainment.ExpiryDate,
                    AttachedDocumentUrl = await DocumentSetting.GetPreSignedUrlAsync(attainment.AttachedDocument),
                    TenantId = attainment.TenantId,
                    IsActive = attainment.ExpiryDate.HasValue ? attainment.ExpiryDate.Value >= currentDate : true
 
                };
                return new ResponseHandler().Success(attainmentDto, "Attainment retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while retrieving attainment with Id: {AttainmentId}. Error: {ErrorMessage}", attainmentId, ex.Message);
                return new ResponseHandler().Exception<AttainmentToReturnDto>("An error occurred while retrieving attainment.");
            }
        }
 
        public async Task<Response<PaginationToReturnDto<AttainmentToReturnDto>>> GetAllAttainmentsAsync(AttainmentDto dto, string tenantId)
        {
            try
            {
                bool isParsed = int.TryParse(tenantId, out int tenantIdInt);
                if (!isParsed)
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<AttainmentToReturnDto>>("Invalid Tenant Id");
 
                (int pageNumber, int pageSize) = GetPaginationParameters(dto.PageNumber, dto.PageSize);
 
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
 
 
                if (!string.IsNullOrWhiteSpace(dto.SearchTerm))
 
                    dto.SearchTerm = dto.SearchTerm.ToLower().Replace(" ", "");
 
 
                Expression<Func<Attainment, bool>> criteria = a => a.TenantId == tenantIdInt &&
                                                                   (string.IsNullOrEmpty(dto.SearchTerm) || a.Title.ToLower().Replace(" ", "").Contains(dto.SearchTerm) || a.IssuingOrganization.ToLower().Replace(" ", "").Contains(dto.SearchTerm)) &&
                                                                   (!dto.IsActive.HasValue ||
                                                                    (dto.IsActive.Value
                                                                        ? (!a.ExpiryDate.HasValue || a.ExpiryDate.Value >= currentDate)
                                                                        : (a.ExpiryDate.HasValue && a.ExpiryDate.Value < currentDate))
                                                                   );
 
                Expression<Func<Attainment, AttainmentToReturnDto>> projection = attainment => new AttainmentToReturnDto
                {
                    Id = attainment.Id,
                    Title = attainment.Title,
                    AttainmentCategoryId = attainment.AttainmentCategoryId,
                    AttainmentCategoryName = attainment.AttainmentCategory.Name,
                    AttainmentCategoryArabicName = attainment.AttainmentCategory.ArabicName,
                    IssuingOrganization = attainment.IssuingOrganization,
                    IssuingDate = attainment.IssuingDate,
                    ExpiryDate = attainment.ExpiryDate,
                    AttachedDocumentUrl = attainment.AttachedDocument,
                    TenantId = attainment.TenantId,
                    IsActive = attainment.ExpiryDate.HasValue ? attainment.ExpiryDate.Value >= currentDate : true
                };
                var spec = new GeneralSpecifications<Attainment>(criteria, pageNumber, pageSize);
 
                spec.AddInclude(iq => iq.Include(a => a.AttainmentCategory));
                spec.AsNoTracking = true;
 
                var attainments = await unitOfWork.Repository<Attainment>().GetAllWithSpecificationsAndProjection(spec, projection);
 
                var tasks = attainments.Select(async attainmentDto =>
                {
                    if (!string.IsNullOrEmpty(attainmentDto.AttachedDocumentUrl))
                    {
                        attainmentDto.AttachedDocumentUrl = await DocumentSetting.GetPreSignedUrlAsync(attainmentDto.AttachedDocumentUrl);
                    }
                });
                await Task.WhenAll(tasks);
 
                var totalCount = await unitOfWork.Repository<Attainment>().GetCountByCriteriaAsync(criteria);
 
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
 
                var attainmentDtos = new PaginationToReturnDto<AttainmentToReturnDto>(totalCount, totalPages, pageSize, pageNumber, attainments);
 
                return new ResponseHandler().Success(attainmentDtos, "Attainments retrieved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while retrieving all attainments. Error: {ErrorMessage}", ex.Message);
                return new ResponseHandler().Exception<PaginationToReturnDto<AttainmentToReturnDto>>("An error occurred while retrieving attainments.");
            }
        }
 
        public async Task<Response<List<object>>> GetAllAttainmentCategoriesAsync()
        {
            try
            {
 
                var spec = new GeneralSpecifications<AttainmentCategory>();
 
                spec.AsNoTracking = true;
                Expression<Func<AttainmentCategory, object>> projection = attainmentCategory => (object)new
                {
                    AttainmentCategoryId = attainmentCategory.Id,
                    AttainmentCategoryName = attainmentCategory.Name,
                    AttainmentCategoryArabicName = attainmentCategory.ArabicName,
                };
                var attainments = await unitOfWork.Repository<AttainmentCategory>().GetAllWithSpecificationsAndProjection(spec, projection);
 
                return new ResponseHandler().Success(attainments);
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while retrieving attainment statistics. Error: {ErrorMessage}", ex.Message);
                return new ResponseHandler().Exception<List<object>>("An error occurred while retrieving attainment statistics.");
            }
        }
        
        private async Task<string> ValidateOnCreationOfAttainment(AddAttainmentDto dto)
        {

            if (dto.ExpiryDate.HasValue && dto.ExpiryDate <= dto.IssuingDate)
            {
                return "Expiry Date must be later than Issuing Date.";
            }

            if (dto.AttachedDocument != null)
            {
                var isValid = DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(dto.AttachedDocument);

                if (!isValid)
                {
                    return "Attached document is either in an invalid format or exceeds the maximum allowed size.";
                }
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return "Title cannot be empty.";
            }

            if (string.IsNullOrWhiteSpace(dto.IssuingOrganization))
            {
                return "Issuing Organization cannot be empty.";
            }


            return string.Empty;
        }

        private async Task<string> ValidateOnAttainmentUpdate(UpdateAttainmentDto dto)
        {
            var attainmentIdIsValid = await unitOfWork.Repository<Attainment>().IsExistsAsync(a => a.Id == dto.Id);

            if (!attainmentIdIsValid)
                return "Invalid Attainment Id";

            var categorytIdIsValid = await unitOfWork.Repository<AttainmentCategory>().IsExistsAsync(ac => ac.Id == dto.AttainmentCategoryId);

            if (!attainmentIdIsValid)
                return "Invalid Attainment Category Id";


            if (dto.ExpiryDate.HasValue && dto.ExpiryDate <= dto.IssuingDate)
            {
                return "Expiry Date must be later than Issuing Date.";
            }

            if (dto.AttachedDocument != null)
            {
                var isValid = DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(dto.AttachedDocument);

                if (!isValid)
                {
                    return "Attached document is either in an invalid format or exceeds the maximum allowed size.";
                }
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return "Title cannot be empty.";
            }

            if (string.IsNullOrWhiteSpace(dto.IssuingOrganization))
            {
                return "Issuing Organization cannot be empty.";
            }


            return string.Empty;
        }

        private (int pageNumber, int pageSize) GetPaginationParameters(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            if (pageSize <= 0 || pageSize > 25)
            {
                pageSize = 25;
            }
            return (pageNumber, pageSize);
        }
    }
}
