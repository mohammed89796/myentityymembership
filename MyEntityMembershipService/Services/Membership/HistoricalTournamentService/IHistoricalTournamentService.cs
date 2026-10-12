using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos;
using MyEntityMembershipService.DTOs.Membership.TournamentResultDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.HistoricalTournamentService
{
    public interface IHistoricalTournamentService
    {
        Task<Response<HistoricalTournamentDto>> AddAsync(CreateHistoricalTournamentDto dto);
        Task<Response<PaginationToReturnDto<HistoricalTournamentDto>>> GetAllAsync(GetHistoricalTournamentParams parameters);
        Task<Response<List<HistoricalTournamentDto>>> GetAllWithoutPaginationAsync();
        Task<Response<HistoricalTournamentDto>> GetByIdAsync(int id);
        Task<Response<bool>> UpdateAsync(int id, HistoricalTournamentUpdateDto dto);
        Task<Response<PaginationToReturnDto<TournamentResultDto>>> GetTournamentResultsAsync(int historicalTournamentId, GetAllTournamentResultsParams parameters);
        Task<Response<PaginationToReturnDto<GetAllHistoricalTournamentResultsDto>>> GetAllHistoricalTournamentResultsAsync(GetAllHistoricalTournamentResultsParams parameters);
        Task<Response<PaginationToReturnDto<HistoricalTournamentResultByMemberDto>>>GetHistoricalResultsByMembershipAsync(HistoricalTournamentResultByMemberParams parameters);
        Task<Response<List<HistoricalTournamentGraphSeriesDto>>> GetHistoricalResultsGraphAsync(HistoricalTournamentGraphParams parameters);
        Task<Response<int>> AddHistoricalTournamentResultAsync(AddHistoricalTournamentResultDto dto);
        Task<Response<GetHistoricalTournamentResultDto>> GetHistoricalTournamentResultByIdAsync(int id);

        Task<Response<string>> SoftDeleteAsync(int id);
        Task<Response<string>> DeleteTournamentResult(int id);

    }


}
