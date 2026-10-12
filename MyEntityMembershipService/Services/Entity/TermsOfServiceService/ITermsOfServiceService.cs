using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Entity.TermsOfServiceDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.TermsOfServiceService;

public interface ITermsOfServiceService
{
    Task<Response<string>> CreateTermsOfService(TermsOfServiceCreateDto dto);
    Task<Response<string>> UpdateTermsOfService(TermsOfServiceUpdateDto dto);
    Task<Response<string>> DeleteTermsOfService(int id, int tenantId);
    Task<Response<TermsOfServiceReadDto>> GetTermsOfService(int id);
    Task<Response<PaginationToReturnDto<TermsOfServiceReadDto>>> GetAllTermsOfService(TermsOfServiceQueryParameters queryParameters);
}