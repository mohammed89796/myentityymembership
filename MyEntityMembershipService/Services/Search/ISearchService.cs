using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.DTOs.Membership;
using MyEntityMembershipService.DTOs.Membership.TempRegisterResponseDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.SearchService
{
    public interface ISearchService
    {
        public Task<Response<List<SearchResultDto>>> MembershipModuleSearch(GenericSearchDto input, string userID);
        public Task<Response<List<SearchResultDto>>> EntityModuleSearch(GenericSearchDto input, string userId);
        public Task<Response<List<SearchResultDto>>> FederationModuleSearch(GenericSearchDto input);
        public Task<Response<List<SearchResultDto>>> SportsClubsModuleSearch(GenericSearchDto input);

    }
}
