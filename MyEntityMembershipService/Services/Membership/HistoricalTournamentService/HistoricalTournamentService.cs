using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos;
using MyEntityMembershipService.DTOs.Membership.TournamentResultDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Membership.HistoricalTournamentService
{
    public class HistoricalTournamentService : IHistoricalTournamentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public readonly IHttpContextAccessor httpContextAccessor;

        public HistoricalTournamentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Response<HistoricalTournamentDto>> AddAsync(CreateHistoricalTournamentDto dto)
        {
            var entity = _mapper.Map<HistoricalTournament>(dto);

            await _unitOfWork.Repository<HistoricalTournament>().Add(entity);
            await _unitOfWork.Save();
            var tournamentDto = _mapper.Map<HistoricalTournamentDto>(entity);


            return new ResponseHandler().Success(tournamentDto);
        }

        public async Task<Response<PaginationToReturnDto<HistoricalTournamentDto>>> GetAllAsync(GetHistoricalTournamentParams parameters)
        {
            var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

            Expression<Func<HistoricalTournament, bool>> criteria = t =>
                  t.GlobalTenantId == tenantId &&
                  (!parameters.Gender.HasValue || t.Gender == parameters.Gender) &&
                  (
                      string.IsNullOrEmpty(parameters.SearchName) ||
                      t.NameEn.ToLower().Contains(parameters.SearchName.ToLower()) ||
                      t.ClubRank.ToString().Contains(parameters.SearchName)
                  ) &&
                  (!parameters.StartDateFrom.HasValue || t.StartDate >= parameters.StartDateFrom.Value) &&
                  (!parameters.StartDateTo.HasValue || t.StartDate <= parameters.StartDateTo.Value) &&
                  (!parameters.RankFrom.HasValue || t.ClubRank >= parameters.RankFrom.Value) &&
                  (!parameters.RankTo.HasValue || t.ClubRank <= parameters.RankTo.Value);

            var specs = new GeneralSpecifications<HistoricalTournament>(
                criteria,
                parameters.PageIndex,
                parameters.PageSize
            )
            {
                Includes = [x => x.Include(t => t.Results)]
            };
            specs.SetOrderByDescending(pb => pb.CreationDate);

            var totalCount = await _unitOfWork.Repository<HistoricalTournament>().GetCountByCriteriaAsync(criteria);

            var tournaments = await _unitOfWork.Repository<HistoricalTournament>().GetAllWithSpecifications(specs);
            var dtos = _mapper.Map<List<HistoricalTournamentDto>>(tournaments).AsReadOnly();

            var pagedResult = new PaginationToReturnDto<HistoricalTournamentDto>(
                count: totalCount,
                totalPages: (int)Math.Ceiling((double)totalCount / parameters.PageSize),
                pageSize: parameters.PageSize,
                pageNumber: parameters.PageIndex,
                data: dtos
            );

            return new ResponseHandler().Success(pagedResult);
        }
        public async Task<Response<List<HistoricalTournamentDto>>> GetAllWithoutPaginationAsync()
        {
            var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

            Expression<Func<HistoricalTournament, bool>> criteria = t =>
                t.GlobalTenantId == tenantId;


            var specs = new GeneralSpecifications<HistoricalTournament>(criteria)
            {
                Includes = [x => x.Include(t => t.Results)]
            };
            specs.SetOrderByDescending(pb => pb.CreationDate);


            var tournaments = await _unitOfWork.Repository<HistoricalTournament>()
                                               .GetAllWithSpecifications(specs);

            var dtos = _mapper.Map<List<HistoricalTournamentDto>>(tournaments);

            return new ResponseHandler().Success(dtos);
        }


        public async Task<Response<HistoricalTournamentDto>> GetByIdAsync(int id)
        {
            var specs = new GeneralSpecifications<HistoricalTournament>(t => t.Id == id)
            {
                Includes = [x => x.Include(t => t.Results)]
            };

            var tournament = await _unitOfWork.Repository<HistoricalTournament>().GetWithSpecifications(specs);
            if (tournament == null)
                return new ResponseHandler().NotFound<HistoricalTournamentDto>("Tournament not found");

            var dto = _mapper.Map<HistoricalTournamentDto>(tournament);
            return new ResponseHandler().Success(dto);
        }

        public async Task<Response<bool>> UpdateAsync(int id, HistoricalTournamentUpdateDto dto)
        {
            var tournament = await _unitOfWork.Repository<HistoricalTournament>().GetByIdAsync(id);
            if (tournament == null)
                return new ResponseHandler().NotFound<bool>("Tournament not found");

            _mapper.Map(dto, tournament);
            _unitOfWork.Repository<HistoricalTournament>().Update(tournament);
            await _unitOfWork.Save();

            return new ResponseHandler().Success(true);
        }

        public async Task<Response<PaginationToReturnDto<TournamentResultDto>>> GetTournamentResultsAsync(int historicalTournamentId, GetAllTournamentResultsParams parameters)
        {
            Expression<Func<TournamentResult, bool>> criteria = r =>
                r.HistoricalTournamentId == historicalTournamentId &&
                (parameters.Races == null || !parameters.Races.Any() || parameters.Races.Contains(r.RaceId.Value)) &&
                (!parameters.Gender.HasValue || r.Membership.Gender == parameters.Gender) &&
                (!parameters.RankFrom.HasValue || r.Ranking >= parameters.RankFrom.Value) &&
                (!parameters.RankTo.HasValue || r.Ranking <= parameters.RankTo.Value) &&
                (
                    string.IsNullOrEmpty(parameters.SearchName) ||
                    (
                        // Athlete full name (ignore spaces + case)
                        (
                            (r.Membership.FirstName + r.Membership.SecondName + r.Membership.MiddleName + r.Membership.LastName)
                                .Replace(" ", "")
                                .ToLower()
                        ).Contains(parameters.SearchName.Replace(" ", "").ToLower())
                    )
                    ||
                    // Lane match
                    r.Lane.Contains(parameters.SearchName)
                );

            var spec = new GeneralSpecifications<TournamentResult>(
                criteria,
                parameters.PageIndex,
                parameters.PageSize
            )
            {
                Includes = [r => r.Include(x => x.Membership)]
            };
            spec.SetOrderByDescending(pb => pb.CreationDate);

            var totalCount = await _unitOfWork.Repository<TournamentResult>()
                .GetCountByCriteriaAsync(criteria);

            var results = await _unitOfWork.Repository<TournamentResult>()
                .GetAllWithSpecifications(spec);

            var resultDtos = _mapper.Map<List<TournamentResultDto>>(results).AsReadOnly();

            var tasks = resultDtos.Select(async dto =>
            {
                dto.Image = await DocumentSetting.GetPreSignedUrlAsync(dto.Image);
            }).ToList();

            await Task.WhenAll(tasks);

            var pagedResult = new PaginationToReturnDto<TournamentResultDto>(
                count: totalCount,
                totalPages: (int)Math.Ceiling((double)totalCount / parameters.PageSize),
                pageSize: parameters.PageSize,
                pageNumber: parameters.PageIndex,
                data: resultDtos
            );

            return new ResponseHandler().Success(pagedResult);
        }



        public async Task<Response<PaginationToReturnDto<GetAllHistoricalTournamentResultsDto>>> GetAllHistoricalTournamentResultsAsync(GetAllHistoricalTournamentResultsParams parameters)
        {
            var globalTenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

            Expression<Func<TournamentResult, bool>> criteria = r =>
                r.GlobalTenantId == globalTenantId &&
                (parameters.Races == null || !parameters.Races.Any() || parameters.Races.Contains(r.RaceId.Value)) &&
                (!parameters.Gender.HasValue || r.Membership.Gender == parameters.Gender) &&
                (!parameters.DateFrom.HasValue || r.RaceDate.Value.ToDateTime(TimeOnly.MinValue) >= parameters.DateFrom.Value) &&
                (!parameters.DateTo.HasValue || r.HistoricalTournament.StartDate <= parameters.DateTo.Value) &&
                (!parameters.RankFrom.HasValue || r.Ranking >= parameters.RankFrom.Value) &&
                (!parameters.RankTo.HasValue || r.Ranking <= parameters.RankTo.Value) &&
                (
                    string.IsNullOrEmpty(parameters.SearchTerm) ||
                    (
                        (
                            (r.Membership.FirstName + r.Membership.SecondName + r.Membership.MiddleName + r.Membership.LastName)
                            .Replace(" ", "").ToLower()
                        ).Contains(parameters.SearchTerm.Replace(" ", "").ToLower()) ||

                        r.HistoricalTournament.NameEn.ToLower().Contains(parameters.SearchTerm.ToLower()) ||

                        r.Lane.ToLower().Contains(parameters.SearchTerm.ToLower())


                    )
                );

            var spec = new GeneralSpecifications<TournamentResult>(
                criteria,
                parameters.PageIndex,
                parameters.PageSize
            )
            {
                Includes =
                [
                    r => r.Include(x => x.Membership),
            r => r.Include(x => x.HistoricalTournament)
                ]
            };
            spec.SetOrderByDescending(pb => pb.CreationDate);

            var totalCount = await _unitOfWork.Repository<TournamentResult>().GetCountByCriteriaAsync(criteria);
            var results = await _unitOfWork.Repository<TournamentResult>().GetAllWithSpecifications(spec);

            var dtos = results.Select(r => new GetAllHistoricalTournamentResultsDto
            {
                Id = r.Id,
                TournamentName = r.HistoricalTournament?.NameEn ?? "N/A",
                AthleteFullName = string.Join(" ", new[]
                {
            r.Membership?.FirstName,
            r.Membership?.SecondName,
            r.Membership?.MiddleName,
            r.Membership?.LastName
        }.Where(n => !string.IsNullOrWhiteSpace(n))),
                Date = r.RaceDate.HasValue ? r.RaceDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                Race = r.RaceName ?? "N/A",
                Time = r.Time ?? TimeSpan.Zero,
                Rank = r.Ranking ?? 0,
                BirthDate = r?.Membership?.BirthDate ?? DateTime.MinValue,
            }).ToList();

            var pagedResult = new PaginationToReturnDto<GetAllHistoricalTournamentResultsDto>(
                count: totalCount,
                totalPages: (int)Math.Ceiling((double)totalCount / parameters.PageSize),
                pageSize: parameters.PageSize,
                pageNumber: parameters.PageIndex,
                data: dtos
            );

            return new ResponseHandler().Success(pagedResult);
        }
        public async Task<Response<PaginationToReturnDto<HistoricalTournamentResultByMemberDto>>> GetHistoricalResultsByMembershipAsync(HistoricalTournamentResultByMemberParams parameters)
        {
            var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

            Expression<Func<TournamentResult, bool>> criteria = r =>
                r.GlobalTenantId == tenantId &&
                r.MembershipId == parameters.MembershipId && r.HistoricalTournament.SportId == parameters.SportId &&
                (parameters.TournamentIds == null || !parameters.TournamentIds.Any() || parameters.TournamentIds.Contains(r.HistoricalTournamentId)) &&
                (parameters.Races == null || !parameters.Races.Any() || parameters.Races.Contains(r.RaceId.Value)) &&
                (!parameters.DateFrom.HasValue || r.RaceDate.Value.ToDateTime(TimeOnly.MinValue) >= parameters.DateFrom.Value) &&
                (!parameters.DateTo.HasValue || r.HistoricalTournament.StartDate <= parameters.DateTo.Value) &&
                (!parameters.RankFrom.HasValue || r.Ranking >= parameters.RankFrom.Value) &&
                (!parameters.RankTo.HasValue || r.Ranking <= parameters.RankTo.Value) &&
                (
                    string.IsNullOrEmpty(parameters.SearchName) ||
                    (
                        (r.Membership.FirstName + r.Membership.SecondName + r.Membership.MiddleName + r.Membership.LastName)
                            .Replace(" ", "").ToLower()
                            .Contains(parameters.SearchName.Replace(" ", "").ToLower()) ||
                        r.HistoricalTournament.NameEn.ToLower().Contains(parameters.SearchName.ToLower()) ||
                        r.Lane.ToLower().Contains(parameters.SearchName.ToLower())
                    )
                );

            var spec = new GeneralSpecifications<TournamentResult>(
                criteria,
                parameters.PageIndex,
                parameters.PageSize
            )
            {
                Includes =
                [
                    r => r.Include(x => x.Membership),
                    r => r.Include(x => x.HistoricalTournament)
                ]
            };
            spec.SetOrderByDescending(pb => pb.CreationDate);


            var totalCount = await _unitOfWork.Repository<TournamentResult>().GetCountByCriteriaAsync(criteria);
            if (totalCount == 0)
            {
                var emptyPaged = new PaginationToReturnDto<HistoricalTournamentResultByMemberDto>(
                    count: 0,
                    totalPages: 0,
                    pageSize: parameters.PageSize,
                    pageNumber: parameters.PageIndex,
                    data: new List<HistoricalTournamentResultByMemberDto>()
                );
                return new ResponseHandler().Success(emptyPaged);
            }
            var results = await _unitOfWork.Repository<TournamentResult>().GetAllWithSpecifications(spec);

            var dtos = results.Select(r => new HistoricalTournamentResultByMemberDto
            {
                Id = r.Id,
                Tournament = r.HistoricalTournament?.NameEn ?? "N/A",
                TournamentId = r.HistoricalTournamentId,
                Date = r.RaceDate.HasValue ? r.RaceDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                Race = r.RaceName ?? "N/A",
                Time = r.Time ?? TimeSpan.Zero,
                Rank = r.Ranking ?? 0,
                Lane = r.Lane ?? "N/A"
            }).ToList();

            var pagedResult = new PaginationToReturnDto<HistoricalTournamentResultByMemberDto>(
                count: totalCount,
                totalPages: (int)Math.Ceiling((double)totalCount / parameters.PageSize),
                pageSize: parameters.PageSize,
                pageNumber: parameters.PageIndex,
                data: dtos
            );

            return new ResponseHandler().Success(pagedResult);
        }

        public async Task<Response<List<HistoricalTournamentGraphSeriesDto>>> GetHistoricalResultsGraphAsync(HistoricalTournamentGraphParams parameters)
        {
            var tenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

            Expression<Func<TournamentResult, bool>> criteria = result =>
                result.GlobalTenantId == tenantId &&
                result.MembershipId == parameters.MembershipId &&
                result.HistoricalTournament.SportId == parameters.SportId &&
                result.RaceDate.HasValue &&
                result.Time.HasValue &&
                (parameters.TournamentIds == null || !parameters.TournamentIds.Any() ||
                    parameters.TournamentIds.Contains(result.HistoricalTournamentId)) &&
                (parameters.RaceIds == null || !parameters.RaceIds.Any() ||
                    (result.RaceId.HasValue && parameters.RaceIds.Contains(result.RaceId.Value))) &&
                (!parameters.DateFrom.HasValue ||
                    result.RaceDate.Value.ToDateTime(TimeOnly.MinValue) >= parameters.DateFrom.Value.Date) &&
                (!parameters.DateTo.HasValue ||
                    result.RaceDate.Value.ToDateTime(TimeOnly.MinValue) <= parameters.DateTo.Value.Date);

            var specification = new GeneralSpecifications<TournamentResult>(criteria)
            {
                Includes =
                [
                    result => result.Include(x => x.HistoricalTournament)
                ]
            };

            var results = await _unitOfWork.Repository<TournamentResult>()
                .GetAllWithSpecifications(specification);

            var graph = results
                .GroupBy(result => new { result.RaceId, Race = result.RaceName ?? "N/A" })
                .Select(group => new HistoricalTournamentGraphSeriesDto
                {
                    RaceId = group.Key.RaceId,
                    Race = group.Key.Race,
                    Points = group
                        .OrderBy(result => result.RaceDate)
                        .ThenBy(result => result.Id)
                        .Select(result => new HistoricalTournamentGraphPointDto
                        {
                            ResultId = result.Id,
                            TournamentId = result.HistoricalTournamentId,
                            Tournament = result.HistoricalTournament?.NameEn ?? "N/A",
                            Date = result.RaceDate!.Value.ToDateTime(TimeOnly.MinValue),
                            Time = result.Time!.Value,
                            TimeInSeconds = result.Time.Value.TotalSeconds,
                            Rank = result.Ranking,
                            Lane = result.Lane ?? "N/A"
                        })
                        .ToList()
                })
                .OrderBy(series => series.Race)
                .ToList();

            return new ResponseHandler().Success(graph);
        }

        public async Task<Response<int>> AddHistoricalTournamentResultAsync(AddHistoricalTournamentResultDto dto)
        {
            var globalTenantId = int.Parse(httpContextAccessor.HttpContext.User.GetTenantId());

            var entity = new TournamentResult
            {
                GlobalTenantId = globalTenantId,
                MembershipId = dto.MembershipId,
                HistoricalTournamentId = dto.HistoricalTournamentId,
                RaceId = dto.RaceId,
                RaceName = dto.RaceName,
                Time = dto.Time,
                Ranking = dto.Rank,
                Lane = dto.Lane,
                RaceDate = dto.Date,
            };

            if (dto != null && dto.Race != null)
            {
                entity.RaceId = dto.Race.RaceId;
                entity.RaceName = dto.Race.RaceName;
            }

            await _unitOfWork.Repository<TournamentResult>().Add(entity);
            await _unitOfWork.Save();

            return new ResponseHandler().Success(entity.Id);
        }

        public async Task<Response<GetHistoricalTournamentResultDto>> GetHistoricalTournamentResultByIdAsync(int id)
        {

            var spec = new GeneralSpecifications<TournamentResult>()
            {

                Includes =
                [
                    r => r.Include(x => x.Membership),
                    r => r.Include(x => x.HistoricalTournament)
                ]
            };
            spec.SetOrderByDescending(pb => pb.CreationDate);

            var entity = await _unitOfWork.Repository<TournamentResult>().GetWithSpecifications(spec);
            if (entity == null)
                return new ResponseHandler().NotFound<GetHistoricalTournamentResultDto>("Result not found");

            var dto = new GetHistoricalTournamentResultDto
            {
                Id = entity.Id,
                AthleteFullName = string.Join(" ", new[] {
                entity.Membership?.FirstName,
                entity.Membership?.SecondName,
                entity.Membership?.MiddleName,
                entity.Membership?.LastName
                }.Where(n => !string.IsNullOrWhiteSpace(n))),
                TournamentName = entity.HistoricalTournament?.NameEn ?? "N/A",
                RaceName = entity.RaceName ?? "N/A",
                Date = entity.HistoricalTournament?.StartDate ?? DateTime.MinValue,
                Time = entity.Time ?? TimeSpan.Zero,
                Rank = entity.Ranking ?? 0,
                Lane = entity.Lane ?? "N/A"
            };

            return new ResponseHandler().Success(dto);
        }

        public async Task<Response<string>> SoftDeleteAsync(int id)
        {
            Expression<Func<HistoricalTournament, bool>> criteria = t => t.Id == id;

            var specs = new GeneralSpecifications<HistoricalTournament>(criteria)
            {
                Includes = [x => x.Include(t => t.Results)]
            };

            var entity = await _unitOfWork.Repository<HistoricalTournament>()
                .GetWithSpecifications(specs);

            if (entity == null || entity.IsDeleted)
                return new ResponseHandler().NotFound<string>("Historical tournament not found");

            if (entity.Results != null && entity.Results.Any())
                _unitOfWork.Repository<TournamentResult>().DeleteRange(entity.Results.ToList());

            _unitOfWork.Repository<HistoricalTournament>().SoftDelete(entity);


            await _unitOfWork.Save();

            return new ResponseHandler().Success("Historical tournament and related results deleted successfully");
        }
        public async Task<Response<string>> DeleteTournamentResult(int id)
        {
            var entity = await _unitOfWork.Repository<TournamentResult>().GetByIdAsync(id);
            if (entity is null || entity.IsDeleted)
                return new ResponseHandler().NotFound<string>("race record not found");

            _unitOfWork.Repository<TournamentResult>().SoftDelete(entity);
            await _unitOfWork.Save();

            return new ResponseHandler().Success("Race record deleted successfully");

        }

    }
}
