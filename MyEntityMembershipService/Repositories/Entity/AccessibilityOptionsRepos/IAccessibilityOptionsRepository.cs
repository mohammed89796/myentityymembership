


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.AccessibilityOptionsRepos
{
    public interface IAccessibilityOptionsRepository : IGenericRepository<NewAccessibilityOptions>
    {
        Task<List<NewAccessibilityOptions>> GetAllAccessibilityOptionsAsync();
    }
}
