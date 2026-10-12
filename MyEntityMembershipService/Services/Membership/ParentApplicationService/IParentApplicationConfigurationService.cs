using MyEntityMembershipService.DTOs.Membership.ParentApplicationConfigurationDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.ParentApplicationService
{
    /// <summary>
    /// Service interface for managing parent application configurations.
    /// </summary>
    public interface IParentApplicationConfigurationService
    {
        Task<Response<string>> CreateParentApplicationConfigAsync(CreateParentApplicationConfigDto createDto);
        Task<Response<string>> UpdateParentApplicationConfigAsync(int id, UpdateParentApplicationConfigDto updateDto);
        Task<Response<ParentApplicationConfigResponseDto>> GetParentApplicationConfigsByPackageIdAsync(int packageId);

    }
}
