using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.FormRepo
{
    public interface IFormRepository : IGenericRepository<Form>
    {
        Task<Form> GetForm(int id);
        Task<List<Form>> GetFormsCreatedBy(string userId, int requestTypeId, string? title);
        Task<List<RequestTypeCategory>> GetRequestTypeCategories();
        Task<List<Form>> GetFormsByTypeId(int requestTypeId,int tenantId);
        Task<List<Form>> GetFormsByTypeAndMemberTypeId(int memberTypeId, int requestTypeId);
        Task<IReadOnlyList<Form>> GetNewRegistrationFormsByRequestIdAsync(int requestTypeId);
        Task<Form> GetNewRegistrationFormsByIdAsync(int Id);
    }
}
