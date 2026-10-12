using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.DTOs.Membership.AreaDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.AreaServ;
using MyEntityMembershipService.Services.Membership.SearchService;
using System.Security.Claims;

namespace MyEntityMembershipService.Controllers.V1.Membership
{
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    // [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }


        [HttpPost("MembershipModuleSearch")]
        public async Task<IResult> MembershipModuleSearch([FromBody] GenericSearchDto input)
        {
            string userID = User.GetUserId();

            var result = await _searchService.MembershipModuleSearch(input, userID);
            return result.GetResponse();
        }
        [HttpPost("EntityModuleSearch")]
        public async Task<IResult> EntityModuleSearch([FromBody] GenericSearchDto input)
        {
            string userID = User.GetUserId();

            var result = await _searchService.EntityModuleSearch(input, userID);
            return result.GetResponse();
        }
        [HttpPost("FederationModuleSearch")]
        public async Task<IResult> FederationModuleSearch([FromBody] GenericSearchDto input)
        {
            var result = await _searchService.FederationModuleSearch(input);
            return result.GetResponse();
        }
        [HttpPost("SportsClubsModuleSearch")]
        public async Task<IResult> SportsClubsModuleSearch([FromBody] GenericSearchDto input)
        {
            var result = await _searchService.SportsClubsModuleSearch(input);
            return result.GetResponse();
        }

    }
}
