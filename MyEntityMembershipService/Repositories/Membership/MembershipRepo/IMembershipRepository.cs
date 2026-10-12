using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.MembershipRepo
{
    public interface IMembershipRepository : IGenericRepository<Data.Membership.Entities.Membership>
    {
        Task<List<Data.Membership.Entities.Membership>> GetUserMemberships(string userId);
        Task<List<Data.Membership.Entities.Membership>> GetUserMembershipsByName(string userId , string name);
        Task<Data.Membership.Entities.Membership> GetMembershipWithSubMembers(int Id);
        Task<List<Data.Membership.Entities.Membership>> GeMembershipByPackageId(int Id);
        Task<Data.Membership.Entities.Membership> GetUserMembershipByPackageId(string userId ,  int PackageId);
        Task<List<Data.Membership.Entities.Membership>> GeMembershipByTenantId(int tenantId);
    }
}
