using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.FederationDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos.MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.NewMembershipDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.FederationMemershipRequestSev;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]

    public class FederationRequestsController : Controller
    {

        private readonly IFederationMemershipRequestService federationMemershipRequestService;

        public FederationRequestsController(IFederationMemershipRequestService federationMemershipRequestService)
        {
            this.federationMemershipRequestService = federationMemershipRequestService;
        }

        [HttpPost("AddAtheleteFormResponse")]
        public async Task<IResult> AddAtheleteFormResponse([FromForm] AddAtheleteFormResponseDto addAtheleteFormResponse)
        {
            var result = await federationMemershipRequestService.AddAtheleteFormResponse(addAtheleteFormResponse);
            return result.GetResponse();
        }

        [HttpPost("AddTransferFormResponse")]
        public async Task<IResult> AddTransferFormResponse([FromForm] AddAtheleteFormResponseDto addAtheleteFormResponse)
        {
            //RequestId for Transfer is 19

            var result = await federationMemershipRequestService.AddTransferOrLoanFormResponse(addAtheleteFormResponse,19);
            return result.GetResponse();
        }
        [HttpPost("AddLoanferFormResponse")]
        public async Task<IResult> AddLoanferFormResponse([FromForm] AddAtheleteFormResponseDto addAtheleteFormResponse)
        {
            //RequestId for Loanfer is 20
            var result = await federationMemershipRequestService.AddTransferOrLoanFormResponse(addAtheleteFormResponse,20);
            return result.GetResponse();
        }
        [HttpPost("AddAthletesFromExcel")]
        public async Task<IResult> AddAthletesFromExcel([FromForm] ExcelAtheleteRowDto excelAtheleteRowDto)
        {
            var result = await federationMemershipRequestService.AddAthletesFromExcelAsync(excelAtheleteRowDto);
            return result.GetResponse();
        }
        [HttpPost("AddFormSubmission")]
        public async Task<IResult> AddFormSubmission([FromForm] FormSubmissionDto formSubmissionDto)
        {
            var result = await federationMemershipRequestService.AddFormSubmissionAsync(formSubmissionDto);
            return result.GetResponse();
        }

        [HttpGet("ValidateAthleteMembership")]
        public async Task<IResult> ValidateAthleteMembership(int areaId, int entityId, string fileNumber, string nationalId)
        {
            var result = await federationMemershipRequestService.ValidateAthleteMembership(areaId,entityId,fileNumber,nationalId);
            return result.GetResponse();
        }
        [HttpPut("update-form-response/{requestId}")]
        public async Task<IResult> UpdateFormResponse(int requestId, [FromForm] List<AtheleteFieldResponseDto> fieldResponseDtos, [FromForm] PackageMembershipRequestAccessDto?  accessDto)
        {
            var result = await federationMemershipRequestService.UpdateFieldResponsesForRequestAsync(requestId, fieldResponseDtos,accessDto);
            return result.GetResponse();
        }
      

    }
}
