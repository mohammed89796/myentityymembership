

using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.AccessibilityOptionsService
{
    public interface IAccessibilityOptionsService
    {
        Task<Response<bool>> CreateAccessibilityOption(string name);
        Task<Response<List<NewAccessibilityOptions>>> GetAccessibilityOptionsDropDown();

    }
}
