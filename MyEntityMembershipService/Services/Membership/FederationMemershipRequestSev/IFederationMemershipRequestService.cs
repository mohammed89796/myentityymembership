
using MyEntityMembershipService.DTOs.Membership.FederationDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos.MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.NewMembershipDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.FederationMemershipRequestSev
{
    public interface IFederationMemershipRequestService
    {
        Task<Response<ReturnAtheleteFormUserResponseDto>> AddAtheleteFormResponse(AddAtheleteFormResponseDto formUserResponseDto);
        Task<Response<ReturnAtheleteFormUserResponseDto>> AddTransferOrLoanFormResponse(AddAtheleteFormResponseDto formUserResponseDto, int requestTypeId);
        Task<Response<MembershipDto>> ValidateAthleteMembership(int areaId, int entityId, string fileNumber, string nationalId);
        Task<Response<MembershipRequestDto>> UpdateFieldResponsesForRequestAsync(int membershipRequestId, List<AtheleteFieldResponseDto> updatedResponses, PackageMembershipRequestAccessDto? requestAccessDto = null);
        Task<Response<List<ReturnAtheleteFormUserResponseDto>>> AddAthletesFromExcelAsync(ExcelAtheleteRowDto sharedDto);
        Task<Response<MembershipRequestDto>> AddFormSubmissionAsync(FormSubmissionDto dto);
    }
}
