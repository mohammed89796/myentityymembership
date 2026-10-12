using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.SearchRepo
{
    public class SearchRepository : ISearchRepository
    {
        private readonly EntityMembershipDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SearchRepository(EntityMembershipDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<SearchResultDto>> SearchEntityAsync(GenericSearchDto input, string userID)
        {
            var keyword = input.Keyword?.Trim().ToLower();
            var results = new List<SearchResultDto>();

            if (input.SearchEntities.Contains("Entity", StringComparer.OrdinalIgnoreCase))
            {
                var query = _context.NewVenues
                    .Include(v => v.Facility)
                        .ThenInclude(f => f.Branch)
                            .ThenInclude(b => b.Tenant)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(v =>
                        v.Name.ToLower().Contains(keyword) ||
                        v.Facility.FacilityName.ToLower().Contains(keyword) ||
                        v.Facility.Branch.Tenant.Name.ToLower().Contains(keyword));
                }

                var venueEntities = await query
                    .Select(v => new SearchResultDto
                    {
                        EntityType = "Entity",
                        Data = new EntityResults
                        {
                            Id = v.Id,
                            ImageUrl = v.VenueImageURL,
                            VenueName = v.Name,
                            BranchName = v.Facility.Branch.Name,
                            FacilityName = v.Facility.FacilityName,
                        }
                    }).ToListAsync();

                results.AddRange(venueEntities);
            }

            return results;
        }

        public async Task<List<SearchResultDto>> SearchMembershipAsync(GenericSearchDto input, string userID)
        {
            var keyword = input.Keyword?.Trim().ToLower();
            var results = new List<SearchResultDto>();

            if (input.SearchEntities == null || !input.SearchEntities.Any() || string.IsNullOrWhiteSpace(userID))
                return results;

            if (input.SearchEntities.Contains("MyMembership", StringComparer.OrdinalIgnoreCase))
            {
                var memberships = await (
                    from m in _context.Memberships
                    join t in _context.Tenants on m.TenantId equals t.Id
                    join p in _context.Packages on m.PackageId equals p.Id into mp
                    from pkg in mp.DefaultIfEmpty()
                    where m.UserId == userID &&
                          (
                              string.IsNullOrWhiteSpace(keyword) ||
                              t.Name.ToLower().Contains(keyword) ||
                              (pkg != null && pkg.Name.ToLower().Contains(keyword))
                          )
                    select new SearchResultDto
                    {
                        EntityType = "MyMembership",
                        Data = new MyMembershipResult
                        {
                            Id = m.Id,
                            TenantName = t.Name,
                            PackageName = pkg.Name,
                            MembershipNumber = m.Id,
                            UserName = $"{m.FirstName} {m.LastName}".Trim(),
                            ParentEntity = t.SuperEntityId != null ? t.SuperEntity.Name : null,
                            DateOfBirth = m.BirthDate,
                            Issue = m.StartDate,
                            NationalID = m.NationalId,
                            Branch = t.Area.Name
                        }
                    }).ToListAsync();

                results.AddRange(memberships);
            }

            if (input.SearchEntities.Contains("MyRequests", StringComparer.OrdinalIgnoreCase))
            {
                var requests = await (
                    from r in _context.MembershipRequests
                    join t in _context.Tenants on r.TenantId equals t.Id
                    join pkg in _context.Packages on r.PackageId equals pkg.Id into packageJoin
                    from p in packageJoin.DefaultIfEmpty()
                    join st in _context.Tenants on t.SuperEntityId equals st.Id into parentJoin
                    from parent in parentJoin.DefaultIfEmpty()
                    where r.UserId == userID &&
                          (
                              string.IsNullOrWhiteSpace(keyword) ||
                              t.Name.ToLower().Contains(keyword) ||
                              (r.FirstName != null && r.FirstName.ToLower().Contains(keyword)) ||
                              (r.LastName != null && r.LastName.ToLower().Contains(keyword)) ||
                              (r.BeltName != null && r.BeltName.ToLower().Contains(keyword)) ||
                              (p != null && p.Name.ToLower().Contains(keyword))
                          )
                    select new SearchResultDto
                    {
                        EntityType = "MyRequests",
                        Data = new MyRequestsResult
                        {
                            Id = r.Id,
                            TenantName = t.Name,
                            PackageName = p.Name ?? "",
                            UserName = $"{r.FirstName} {r.LastName}".Trim(),
                            RequestType = r.RequestType.Name,
                            ParentEntity = t.SuperEntity.Name,
                            DateOfSubmittion = r.CreationDate,
                            RequestStatusId = (int)r.Status,
                            RequestStatus = r.Status.ToString()
                        }
                    }).ToListAsync();

                results.AddRange(requests);
            }

            return results;
        }

        public async Task<List<SearchResultDto>> SearchFederationAsync(GenericSearchDto input)
        {
            var keyword = input.Keyword?.Trim().ToLower();
            var results = new List<SearchResultDto>();

            if (input.SearchEntities == null || !input.SearchEntities.Any())
                return results;

            if (input.SearchEntities.Contains("Federation", StringComparer.OrdinalIgnoreCase))
            {
                var query = _context.Tenants
                    .Where(t => t.SuperEntityId == null);

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(t => t.Name.ToLower().Contains(keyword));
                }

                var federations = await query
                    .Select(t => new SearchResultDto
                    {
                        EntityType = "Federation",
                        Data = new FederationResults
                        {
                            Id = t.Id,
                            FedName = t.Name,
                            FedImage = t.Image,
                            Location = t.Location
                        }
                    }).ToListAsync();

                results.AddRange(federations);
            }

            return results;
        }

        public async Task<List<SearchResultDto>> SearchClubsAsync(GenericSearchDto input)
        {
            var keyword = input.Keyword?.Trim().ToLower();
            var results = new List<SearchResultDto>();

            if (input.SearchEntities.Contains("Clubs", StringComparer.OrdinalIgnoreCase))
            {
                var query = _context.Tenants
                    .Where(t => t.SuperEntityId == null)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(v => v.Name.ToLower().Contains(keyword));
                }

                var entities = await query
                    .Select(v => new SearchResultDto
                    {
                        EntityType = "Clubs",
                        Data = new SportsClubsResults
                        {
                            Id = v.Id,
                            ClubName = v.Name,
                            ClubImage = v.Image,
                            Location = v.Location
                        }
                    }).ToListAsync();

                results.AddRange(entities);
            }

            return results;
        }
        public async Task<List<SearchResultDto>> SearchPackagesAsync(GenericSearchDto input)
        {
            var keyword = input.Keyword?.Trim().ToLower();
            var results = new List<SearchResultDto>();

            if (input.SearchEntities.Contains("Packages", StringComparer.OrdinalIgnoreCase))
            {
                var query = _context.Packages.AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(p => p.Name.ToLower().Contains(keyword));
                }

                var entities = await query
                    .Select(p => new SearchResultDto
                    {
                        EntityType = "Packages",
                        Data = new PackagesResults
                        {
                            Id = p.Id,
                            PackageImage = p.Name,
                            PackageName = p.Image,
                            Price = p.Price,
                            EntityName = p.Tenant.Name
                        }
                    }).ToListAsync();

                results.AddRange(entities);
            }

            return results;
        }
    }
}
