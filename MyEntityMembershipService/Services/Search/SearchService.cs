using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.Entity.SearchRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;

namespace MyEntityMembershipService.Services.Membership.SearchService
{

    public class SearchService : ISearchService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ISearchRepository _searchRepository;

        public SearchService(IUnitOfWork unitOfWork, ISearchRepository searchRepository)
        {
            _unitOfWork = unitOfWork;
            _searchRepository = searchRepository;
        }


        public async Task<Response<List<SearchResultDto>>> MembershipModuleSearch(GenericSearchDto input, string userId)
        {
            try
            {
                var result = await _searchRepository.SearchMembershipAsync(input, userId);

                return new ResponseHandler().Success<List<SearchResultDto>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<SearchResultDto>>("Something went wrong");
            }
        }

        public async Task<Response<List<SearchResultDto>>> EntityModuleSearch(GenericSearchDto input, string userId)
        {
            try
            {
                var result = await _searchRepository.SearchEntityAsync(input, userId);

                var entityResults = result
                        .Select(r => r.Data)
                        .OfType<EntityResults>()
                        .ToList();


                var tasks = entityResults.Select(async er =>
                {
                    er.ImageUrl = await DocumentSetting.GetPreSignedUrlAsync(er.ImageUrl);
                });
                await Task.WhenAll(tasks);

                return new ResponseHandler().Success<List<SearchResultDto>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<SearchResultDto>>("Something went wrong");
            }
        }
        public async Task<Response<List<SearchResultDto>>> FederationModuleSearch(GenericSearchDto input)
        {
            try
            {
                var result = await _searchRepository.SearchFederationAsync(input);

                var federationResults = result
                       .Select(r => r.Data)
                       .OfType<FederationResults>()
                       .ToList();

                var tasks = federationResults.Select(async fr =>
                {
                    fr.FedImage = await DocumentSetting.GetPreSignedUrlAsync(fr.FedImage);
                });

                await Task.WhenAll(tasks);

                return new ResponseHandler().Success<List<SearchResultDto>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<SearchResultDto>>("Something went wrong");
            }
        }
        public async Task<Response<List<SearchResultDto>>> SportsClubsModuleSearch(GenericSearchDto input)
        {
            try
            {
                var result = await _searchRepository.SearchClubsAsync(input);

                var sportsClubsResults = result
                       .Select(r => r.Data)
                       .OfType<SportsClubsResults>()
                       .ToList();

                var tasks = sportsClubsResults.Select(async sr =>
                {
                    sr.ClubImage = await DocumentSetting.GetPreSignedUrlAsync(sr.ClubImage);
                });

                await Task.WhenAll(tasks);
                return new ResponseHandler().Success<List<SearchResultDto>>(result);
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<List<SearchResultDto>>("Something went wrong");
            }
        }
    }
}
