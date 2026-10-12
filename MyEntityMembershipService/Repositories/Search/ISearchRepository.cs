


using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.SearchRepo
{
    public interface ISearchRepository
    {
        public Task<List<SearchResultDto>> SearchMembershipAsync(GenericSearchDto input, string userID);
        public Task<List<SearchResultDto>> SearchEntityAsync(GenericSearchDto input, string userID);
        public Task<List<SearchResultDto>> SearchFederationAsync(GenericSearchDto input);
        public Task<List<SearchResultDto>> SearchClubsAsync(GenericSearchDto input);

    }
}
