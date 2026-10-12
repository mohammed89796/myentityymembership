using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.Repositories.Common;
namespace MyEntityMembershipService.Services.Entity.SeriesService
{
    public interface ISeriesService
    {
        Task<Response<SeriesToReturnDto>> CreateSeries(SeriesDto seriesDto);

        Task<Response<SeriesToReturnDto>> UpdateSeries(UpdateSeriesDto seriesDto, int seriesId);

        Task<Response<SeriesToReturnDto>> DeleteSeries(int seriesId);

        Task<Response<SeriesToReturnDto>> GetSeriesById(int seriesId);

        Task<Response<object>> GetAllSeries(string? search);

        Task<Response<IReadOnlyList<SeriesToReturnDto>>> FilterSeries(FilterSeriesDto filters);

        Task<Response<IReadOnlyList<SeriesToReturnDto>>> SortSeries(string sort);
    }
}
