using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Entity.SeriesService;

namespace MyEntityMembershipService.Controllers.V1.Entity
{

    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize("ClientIdPolicy")]
    public class SeriesController : ControllerBase
    {
        private readonly ISeriesService _seriesService;

        public SeriesController(ISeriesService seriesService)
        {
            _seriesService = seriesService;
        }


        [HttpPost("AddSeries")]
        public async Task<IResult> CreateSeries([FromForm] SeriesDto seriesDto)

            => (await _seriesService.CreateSeries(seriesDto)).GetResponse();


        [HttpPut("UpdateSeries/{seriesId}")]
        public async Task<IResult> UpdateSeries([FromForm] UpdateSeriesDto seriesDto, [FromRoute] int seriesId)

            => (await _seriesService.UpdateSeries(seriesDto, seriesId)).GetResponse();


        [HttpDelete("DeleteSeries/{seriesId}")]
        public async Task<IResult> DeleteSeries(int seriesId)

            => (await _seriesService.DeleteSeries(seriesId)).GetResponse();


        [HttpGet("GetSeries/{seriesId}")]
        public async Task<IResult> GetSeries(int seriesId)

            => (await _seriesService.GetSeriesById(seriesId)).GetResponse();


        [HttpGet("GetAllSeries")]
        public async Task<IResult> GetAllSeries([FromQuery] string? search)

            => (await _seriesService.GetAllSeries(search)).GetResponse();


        [HttpPost("FilterSeries")]
        public async Task<IResult> FilterSeries([FromBody] FilterSeriesDto filterAllSeriesDto)

            => (await _seriesService.FilterSeries(filterAllSeriesDto)).GetResponse();


        [HttpGet("SortSeries")]
        public async Task<IResult> SortSeries([FromQuery] string sort)

            => (await _seriesService.SortSeries(sort)).GetResponse();
    }
}
