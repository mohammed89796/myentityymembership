using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.FormServ;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class FormController : ControllerBase
    {
        private readonly IFormService formService;

        public FormController(IFormService formService)
        {
            this.formService = formService;
        }
        [HttpGet("GetRequestTypeFields")]
        public async Task<IResult> GetRequestTypeFields(int requestTypeId)
        {
            var result = await formService.GetRequestTypeFields(requestTypeId);
            return result.GetResponse();
        }
        [HttpPost("CreateUpdateForm")]
        public async Task<IResult> CreateUpdateForm(CreateUpdateFormDto freezFormDto)
        {
            freezFormDto.TenantId = User.GetParsedTenantId();
            var result = await formService.CreateUpdateForm(freezFormDto);
            return result.GetResponse();
        }
        
        [HttpGet("GetForm")]
        public async Task<IResult> GetForm(int id, int? packageId , int? memberTypeId)
        {
            var result = await formService.GetForm(id, packageId, memberTypeId);
            return result.GetResponse();
        }
        [HttpGet("GetDraftForm")]
        public async Task<IResult> GetDraftForm(int id, int? packageId)
        {
            var result = await formService.GetDraftForm(id, packageId);
            return result.GetResponse();
        }
        [HttpGet("GetFormsCreatedBy")]
        public async Task<IResult> GetFormsCreatedBy(string userId, int requestTypeId, string? title)
        {
            var tenantId = User.GetParsedTenantId();
            var userName = User.GetUserName();
            var result = await formService.GetFormsCreatedBy(userId, requestTypeId, title,tenantId);
            return result.GetResponse();
        }
        [HttpGet("GetRequestTypesGroupedByCategories")]
        public async Task<IResult> GetRequestTypesGroupedByCategories()
        {
            var tenantId = User.GetParsedTenantId();
            var result = await formService.GetRequestTypesGroupedByCategories(tenantId);
            return result.GetResponse();
        }
        [HttpDelete("DeleteForm")]
        public async Task<IResult> DeleteForm(int id)
        {
            var result = await formService.DeleteForm(id);
            return result.GetResponse();
        }
        [HttpPost("AddFormUserResponse")]
        public async Task<IResult> AddFormUserResponse(FormUserResponseDto formUserResponseDto)
        {
            var result = await formService.AddFormUserResponse(formUserResponseDto);
            return result.GetResponse();
        }
        [HttpGet("GetFormByRequestTypeMemberTypeId")]
        public async Task<IResult> GetFormByRequestTypeMemberTypeId(int memberTypeId, int requestTypeId)
        {
            var result = await formService.GetFormsByMemberTypeAndRequestType(memberTypeId, requestTypeId);
            return result.GetResponse();
        }
        [HttpPost("UploadFile")]
        public async Task<IResult> UploadFile([FromForm] UploadFileDto fileDto)
        {
            var result = await formService.UploadFile(fileDto.file);
            return result.GetResponse();
        }

        [HttpGet("GetFormByPackgeAndRequestTypeAndMemberTypeId")]
        public async Task<IResult> GetFormBypackgeRequestTypeMemberTypeId(int packageId, int requestTypeId)
        {
            var result = await formService.GetFormsForEachSubMemberAsync(packageId, requestTypeId);
            return result.GetResponse();
        }
        [HttpGet("GetFormByPackgeAndRequestTypeAndMemberType")]
        public async Task<IResult> GetFormBypackgeRequestTypeMemberTypeId(int packageId, int requestTypeId, int? memberTypeId)
        {
            var result = await formService.GetFormsForSubMemberAsync(packageId, requestTypeId, memberTypeId);
            return result.GetResponse();
        }

        [HttpGet("GetConfiguration")]
        public async Task<IResult> GetConfiguration(int packageId, int requestTypeId)
        {
            var result = await formService.GetPackageDetailsByRequestTypeAndPackageID(requestTypeId, packageId);
            return result.GetResponse();
        }


        [HttpGet("GetNewRegistrationForms")]
        public async Task<IResult> GetNewRegistrationForms()

            => (await formService.GetNewRegistrationFormsByRequestTypeId()).GetResponse();

        [HttpGet("GetNewRegistrationFormById/{Id}")]
        public async Task<IResult> GetNewRegistrationForms(int Id)

            => (await formService.GetNewRegistrationFormsById(Id)).GetResponse();
        [HttpGet("CheckCanSelectPackage")]
        public async Task<IResult> CheckCanSelectPackage(string userId , int packageId)
        {
            var result = await formService.CheckCanSelectPackage(packageId, userId);
            return result.GetResponse();
        }
        [HttpGet("GetFreezCard")]
        public async Task<IResult> GetFreezCard(int membershipId)
        {
            var result = await formService.GetFreezCard(membershipId);
            return result.GetResponse();
        }
        [HttpPost("CheckExceedWithAdditionalPayment")]
        public async Task<IResult> CheckExceedWithAdditionalPayment(FormUserResponseDto formUserResponseDto)
        {
            var result = await formService.CheckExceedWithAdditionalPayment(formUserResponseDto);
            return result.GetResponse();
        }


        [HttpGet("GetAllForms/{requestTypeId}")]
        public async Task<IResult> GetAllFormsByTenantId(int requestTypeId, [FromQuery] FormResponseSearchDto dto)
        {
            dto.TenantId = User.GetParsedTenantId();
            var result = await formService.GetAllFormsByTenantIdAsync(requestTypeId, dto);
            return result.GetResponse();
        }
        
    }
}
