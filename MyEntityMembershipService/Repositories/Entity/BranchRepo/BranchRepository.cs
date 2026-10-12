using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.BranchRepo
{
    public class BranchRepository : GenericRepository<Branch>, IBranchRepository
    {
        private readonly EntityMembershipDbContext _context;

        public BranchRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Branch>> GetAllBranchsAsync()
        {
            return await _context
                .Branches.AsNoTracking()
                .Include(b => b.NewFacilities)
                .ThenInclude(f => f.NewVenues)
                .ToListAsync();
        }

        public async Task<Branch> GetBranchByIdAsync(int id)
        {
            return await _context
                .Branches
                .Include(b => b.NewFacilities)
                .ThenInclude(f => f.NewVenues)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Branch>> GetBranchByTenantIdAsync(int tenantId,string? searchTerm, int? pageIndex, int? pageSize)
        {
            searchTerm = searchTerm?.Trim().ToLower();
            var query = _context.Branches
                .Include(b => b.NewFacilities)
                .ThenInclude(f => f.NewVenues)
                .Where(t => tenantId == 1 || t.TenantId == tenantId && (string.IsNullOrWhiteSpace(searchTerm) || t.Name.Trim().ToLower().Contains(searchTerm)));

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                query = query
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }
            return await query.ToListAsync();


        }

        public async Task<IEnumerable<int>> GetVenuesOfTenantByIdAsync(int tenantId)
        {
            return await _context.Branches
                .AsNoTracking()
                .Where(t => t.TenantId == tenantId)
                .Include(b => b.NewFacilities)
                .ThenInclude(f => f.NewVenues)
                .SelectMany(b => b.NewFacilities.SelectMany(nf => nf.NewVenues.Select(nv => nv.Id)))
                .ToListAsync();
        }



        public async Task<BranchDetailsDto> GetBranchDetailsByIdAsync(int branchID)
        {
            var branchOperatingHours = await _context.BranchOperatingHours.FirstOrDefaultAsync(oh => oh.BranchID == branchID);
            return await _context
                .Branches
                .Where(b => b.Id == branchID)
                .Include(b => b.NewFacilities)
                .Include(b => b.BranchLocation)
                .Include(b => b.BranchImages)
                .Include(b => b.BranchOperatingHours)
                .Include(b => b.BranchServices)
                    .ThenInclude(bs => bs.CoreService)
                .Select(b => new BranchDetailsDto
                {
                    Id = b.Id,
                    ImageURL = b.BranchImageURL,
                    BranchName = b.Name,
                    Description = b.Description,
                    NumberOfFacilities = b.NewFacilities.Count,
                    BranchImages = b.BranchImages.Where(e => e.BranchID == branchID).ToList(),
                    BranchInformation = new BranchInformationDto
                    {
                        Country =
                            _context
                                .Country.Where(c => c.Id == b.CountryID)
                                .Select(c => c.Name)
                                .FirstOrDefault() ?? "Unknown",
                        State =
                            _context
                                .State.Where(s => s.Id == b.StateID)
                                .Select(s => s.Name)
                                .FirstOrDefault() ?? "Unknown",
                        City =
                            _context
                                .City.Where(c => c.Id == b.CityID)
                                .Select(c => c.Name)
                                .FirstOrDefault() ?? "Unknown",
                        StreetAddress = b.Location ?? "No Address",
                        PostalCode = b.PostalCode,
                        BranchManagerName = "Mostafa", // FIXME: Replace with actual name
                        BranchServices = b.BranchServices
                            .Select(bs => bs.CoreService.Name)
                            .ToList()
                    },
                    LocationDetails = new BranchLocationDetailsDto
                    {
                        Name = b.BranchLocation.Name ?? "Unknown Location",
                        Latitude = b.BranchLocation.Latitude ?? "0",
                        Longitude = b.BranchLocation.Longitude ?? "0",
                    },
                    ContactInfo = new BranchContactInfoDto
                    {
                        Email = b.EmailAddress,
                        PhoneNumber = $"{b.CountryCodeID} {b.PhoneNumber}"
                    },
                    OperatingHours = branchOperatingHours
                })
                .FirstOrDefaultAsync();
        }


        public async Task<BranchListDetailsDto> GetBranchCardsAsync(int pageNumber, int tenantID)
        {
            int pageSize = 8;

            int totalNumber = await _context.Branches.Where(b => b.TenantId == tenantID).CountAsync();

            var branches = await _context
                .Branches
               .Where(b => b.TenantId == tenantID)
                .Include(b => b.NewFacilities)
                .Include(b => b.BranchLocation)
                .Select(b => new BranchCardViewDto
                {
                    Id = b.Id,
                    ImageURL = b.BranchImageURL,
                    BranchName = b.Name,
                    Address = b.Location
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalPages = (int)Math.Ceiling((double)totalNumber / pageSize);

            return new BranchListDetailsDto
            {
                TotalNumber = totalNumber,
                NumberOfPages = totalPages,
                Branches = branches
            };
        }
        public async Task<EditBranchDetailsDto> GetBranchEditDetailsByIdAsync(int branchID)
        {
            var branchDetailsDto = await _context.Branches
                .Where(b => b.Id == branchID)
                .Select(b => new EditBranchDetailsDto
                {
                    ID = b.Id,
                    BranchImageURL = b.BranchImageURL,
                    BranchName = b.Name,
                    AddressDetails = new BranchCountryDetailsDto
                    {
                        CountryID = b.CountryID,
                        CountryName = _context.Country
                            .Where(c => c.Id == b.CountryID)
                            .Select(c => c.Name)
                            .FirstOrDefault(),
                        StateID = b.StateID,
                        StateName = _context.State
                            .Where(s => s.Id == b.StateID)
                            .Select(s => s.Name)
                            .FirstOrDefault(),
                        CityID = b.CityID,
                        CityName = _context.City
                            .Where(city => city.Id == b.CityID)
                            .Select(city => city.Name)
                            .FirstOrDefault()
                    },
                    Location = b.Location,
                    PostalCode = b.PostalCode,
                    BranchManagerID = b.BranchManagerID,
                    BranchManagerName = "Mushtaha",
                    BranchServices = b.BranchServices.Select(s => new BranchServicesDetailsDto
                    {
                        ServiceID = s.CoreServiceID,
                        ServiceName = s.CoreService.Name
                    }).ToList(),
                    Description = b.Description,
                    ContactInformation = new EditBranchContactInfoDto
                    {
                        CountryCodeID = _context.CountryCodes.Where(cc => cc.Code == b.CountryCodeID).Select(cc => cc.Id).FirstOrDefault(),
                        CountryCode = b.CountryCodeID,
                        PhoneNumber = b.PhoneNumber,
                        EmailAddress = b.EmailAddress
                    },
                    BranchLocation = new EditBranchLocationDto
                    {
                        Name = b.BranchLocation.Name,
                        Latitude = b.BranchLocation.Latitude,
                        Longitude = b.BranchLocation.Longitude
                    },
                    BranchMedia = b.BranchImages.Select(img => new EditBranchMediaDto
                    {
                        ID = img.Id,
                        Link = img.ImageUrl
                    }).FirstOrDefault(),
                    BranchOperatingHours = b.BranchOperatingHours
                })
                .FirstOrDefaultAsync();

            return branchDetailsDto;
        }
        public async Task<BranchMobileCardListDto> GetBranchCardsByTenantID(int pageNumber, int tenantID)
        {
            int pageSize = 8;

            int totalNumber = await _context.Branches
                .Where(b => b.TenantId == tenantID)
                .CountAsync();

            int totalPages = (int)Math.Ceiling((double)totalNumber / pageSize);

            var branches = await _context.Branches
                .Where(b => b.TenantId == tenantID)
                // .OrderBy(b => b.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new
                {
                    b.Id,
                    b.BranchImageURL,
                    b.Name,
                    b.Location,
                    FacilityRatings = b.NewFacilities
                        .SelectMany(f => f.NewVenues)
                        .SelectMany(v => v.VenueReviews)
                        .Select(vr => vr.Rating)
                })
                .ToListAsync();

            var branchCards = branches.Select(b => new BranchMobileCardViewDto
            {
                Id = b.Id,
                ImageURL = b.BranchImageURL,
                BranchName = b.Name,
                Address = b.Location,
                Rating = b.FacilityRatings.Any() ? b.FacilityRatings.Average() : 0.0
            }).ToList();

            var result = new BranchMobileCardListDto
            {
                TotalNumber = totalNumber,
                NumberOfPages = totalPages,
                Branches = branchCards
            };

            return result;
        }

        public async Task<BranchMobileDetailsDto> GetBranchDetailsMobileView(int branchID)
        {
            var branchDetails = await _context.Branches
                .Where(b => b.Id == branchID)
                .Select(b => new BranchMobileDetailsDto
                {
                    Id = b.Id,
                    ImageURL = b.BranchImageURL,
                    BranchName = b.Name,
                    Address = b.Location,

                    BranchAboutView = new BranchMobileAboutDto
                    {
                        Description = b.Description,
                        Governorate = _context
                                .State.Where(s => s.Id == b.StateID)
                                .Select(s => s.Name)
                                .FirstOrDefault() ?? "Unknown", // Assuming State.Name holds the governorate information
                        PostalCode = b.PostalCode,
                        BranchManagerName = "Ali Mansour",
                        BranchServices = b.BranchServices.Select(s => s.CoreService.Name).ToList()
                    },

                    BranchInformationView = new BranchMobileInformationDto
                    {
                        Country = _context
                                .Country.Where(c => c.Id == b.CountryID)
                                .Select(c => c.Name)
                                .FirstOrDefault() ?? "Unknown",
                        PhoneNumber = $"{b.CountryCodeID} {b.PhoneNumber}",
                        Email = b.EmailAddress
                    },

                    LocationDetails = new BranchMobileLocationDetailsDto
                    {
                        Name = b.BranchLocation.Name,
                        Latitude = b.BranchLocation.Latitude,
                        Longitude = b.BranchLocation.Longitude
                    },
                    BranchMedia = b.BranchImages.Select(img => new BranchMobileMediaDto
                    {
                        ID = img.Id,
                        ImageURL = img.ImageUrl
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return branchDetails;
        }
        public async Task<List<MobileBranchFacilitiesDto>> GetMobileBranchFacilitiesAsync(int branchID)
        {
            var facilities = await _context.NewFacility
                .Where(f => f.BranchId == branchID)
                .Select(f => new
                {
                    f.Id,
                    f.FacilityImageUrl,
                    f.FacilityName,
                    FacilityType = f.FacilityType.name,
                    SportsList = f.SportsList.Where(s => !s.IsDeleted).Select(s => s.Sports.SportName).ToList(),
                    VenueRatings = f.NewVenues.SelectMany(v => v.VenueReviews).Select(vr => vr.Rating)
                })
                .ToListAsync();

            var mobileBranchFacilities = facilities.Select(f => new MobileBranchFacilitiesDto
            {
                Id = f.Id,
                ImageURL = f.FacilityImageUrl,
                FacilityName = f.FacilityName,
                FacilityType = f.FacilityType,
                SportsCount = f.SportsList.Count(),
                Sports = f.SportsList.Take(4).ToList(),
                Rating = f.VenueRatings.Any() ? f.VenueRatings.Average() : 0.0
            }).ToList();

            return mobileBranchFacilities;
        }



    }
}
