using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;



using MyEntityMembershipService.DTOs.Entity.FacilityDtos;

using MyEntityMembershipService.Repositories.GenericRepo;


namespace MyEntityMembershipService.Repositories.Entity.FacilityRepo
{
    public class FacilityRepository : GenericRepository<NewFacility>, IFacilityRepository
    {
        private readonly EntityMembershipDbContext _context;

        public FacilityRepository(EntityMembershipDbContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<FacilityListDetailsDto> GetFacilitiesByBranchIdAsync(int pageNumber, int branchID)
        {
            int pageSize = 8;

            int totalNumber = await _context.NewFacility
                .Where(f => f.BranchId == branchID)
                .CountAsync();

            var facilities = await _context.NewFacility
                .Where(f => f.BranchId == branchID)
                .Include(f => f.Branch)
                .Select(f => new FacilityCardViewDto
                {
                    Id = f.Id,
                    ImageURL = f.FacilityImageUrl,
                    FacilityName = f.FacilityName,
                    Address = (from branch in _context.Branches
                               join country in _context.Country on branch.CountryID equals country.Id
                               join state in _context.State on branch.StateID equals state.Id
                               join city in _context.City on branch.CityID equals city.Id
                               where branch.Id == f.BranchId
                               select $"{country.Name}, {state.Name}").FirstOrDefault(),
                    PhoneNumber = $"{f.CountryCodeID} {f.PhoneNumber}",
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalPages = (int)Math.Ceiling((double)totalNumber / pageSize);

            return new FacilityListDetailsDto
            {
                TotalNumber = totalNumber,
                NumberOfPages = totalPages,
                Facilities = facilities
            };
        }

        public async Task<FacilityDetailsDto> GetFacilityDetailsByIdAsync(int facilityId)
        {
            return await _context
                .NewFacility
                .Where(f => f.Id == facilityId)
                .Include(f => f.FacilityLocation)
                .Include(f => f.FacilityOperatingHours)
                .Include(f => f.FacilityUploads)
                .Include(f => f.FacilityAmenities)
                    .ThenInclude(fa => fa.Amenity)
                .Include(f => f.FacilityAccessibilityOptions)
                    .ThenInclude(fao => fao.AccessibilityOptions)
                .Include(f => f.SportsList)
                    .ThenInclude(fs => fs.Sports)
                .Include(f => f.Branch)
                .Select(f => new FacilityDetailsDto
                {
                    FacilityID = f.Id,
                    HeaderDetails = new FacilityHeaderDetailsDto
                    {
                        FacilityImageURL = f.FacilityImageUrl,
                        FacilityName = f.FacilityName,
                        BranchName = f.Branch.Name,
                        BranchImageURL = f.Branch.BranchImageURL,
                        Description = f.Description,
                        FacilityTypeName = _context.FacilityType
                            .Where(ft => ft.Id == f.FacilityTypeId)
                            .Select(ft => ft.name)
                            .FirstOrDefault() ?? "Unknown",
                        Email = f.Branch.EmailAddress,
                        Address = string.Concat(
                            _context.Country
                                .Where(c => c.Id == f.Branch.CountryID)
                                .Select(c => c.Name)
                                .FirstOrDefault() ?? "Unknown Country", ", ",
                            _context.State
                                .Where(s => s.Id == f.Branch.StateID)
                                .Select(s => s.Name)
                                .FirstOrDefault() ?? "Unknown State"
                        ),
                        Country = _context.Country
                            .Where(c => c.Id == f.Branch.CountryID)
                            .Select(c => c.Name)
                            .FirstOrDefault() ?? "Unknown Country",
                        PhoneNumber = $"{f.CountryCodeID} {f.PhoneNumber}",
                        NumberOfVenues = f.NewVenues.Count
                    },
                    FeaturesDetails = new FacilityFeaturesDto
                    {
                        FacilitySports = f.SportsList
                            .Select(fs => fs.Sports.SportName)
                            .ToList(),
                        FacilityAccessibility = f.FacilityAccessibilityOptions
                            .Select(fao => fao.AccessibilityOptions.OptionName)
                            .ToList(),
                        FacilityAmenities = f.FacilityAmenities
                            .Select(fa => fa.Amenity.name)
                            .ToList()
                    },
                    OperatingHours = new OperatingHoursDto
                    {
                        SundayOpenAt = f.FacilityOperatingHours.sundayOpenAt,
                        SundayCloseAt = f.FacilityOperatingHours.sundayCloseAt,
                        SundayIsWorking = f.FacilityOperatingHours.sundayIsWorking ?? false,
                        MondayOpenAt = f.FacilityOperatingHours.mondayOpenAt,
                        MondayCloseAt = f.FacilityOperatingHours.mondayCloseAt,
                        MondayIsWorking = f.FacilityOperatingHours.mondayIsWorking ?? false,
                        TuesdayOpenAt = f.FacilityOperatingHours.tuesdayOpenAt,
                        TuesdayCloseAt = f.FacilityOperatingHours.tuesdayCloseAt,
                        TuesdayIsWorking = f.FacilityOperatingHours.tuesdayIsWorking ?? false,
                        WednesdayOpenAt = f.FacilityOperatingHours.wednesdayOpenAt,
                        WednesdayCloseAt = f.FacilityOperatingHours.wednesdayCloseAt,
                        WednesdayIsWorking = f.FacilityOperatingHours.wednesdayIsWorking ?? false,
                        ThursdayOpenAt = f.FacilityOperatingHours.thursdayOpenAt,
                        ThursdayCloseAt = f.FacilityOperatingHours.thursdayCloseAt,
                        ThursdayIsWorking = f.FacilityOperatingHours.thursdayIsWorking ?? false,
                        FridayOpenAt = f.FacilityOperatingHours.fridayOpenAt,
                        FridayCloseAt = f.FacilityOperatingHours.fridayCloseAt,
                        FridayIsWorking = f.FacilityOperatingHours.fridayIsWorking ?? false,
                        SaturdayOpenAt = f.FacilityOperatingHours.saturdayOpenAt,
                        SaturdayCloseAt = f.FacilityOperatingHours.saturdayCloseAt,
                        SaturdayIsWorking = f.FacilityOperatingHours.saturdayIsWorking ?? false
                    },
                    FacilityImages = f.FacilityUploads
                        .Where(e => e.FacilityID == facilityId)
                        .ToList(),
                    LocationDetails = new FacilityLocationDetailsDto
                    {
                        Name = f.FacilityLocation.Name ?? "Unknown Location",
                        Latitude = f.FacilityLocation.Latitude ?? "0",
                        Longitude = f.FacilityLocation.Longitude ?? "0"
                    }
                })
                .FirstOrDefaultAsync();
        }
        public async Task<EditFacilityDetailsDto> GetFacilityEditDetailsByIdAsync(int facilityID)
        {
            var editFacilityDto = await _context.NewFacility
                .Where(f => f.Id == facilityID)
                .Select(f => new EditFacilityDetailsDto
                {
                    Id = f.Id,
                    BranchId = f.BranchId,
                    FacilityName = f.FacilityName,
                    FacilityImageUrl = f.FacilityImageUrl,
                    FacilityTypeId = f.FacilityTypeId,
                    Description = f.Description,
                    CountryCodeID = f.CountryCodeID,
                    PhoneNumber = f.PhoneNumber,
                    FacilityPremisesTypeID = f.FacilityPremisesTypeID,

                    FacilityLocation = new EditFacilityLocationDto
                    {
                        Name = f.FacilityLocation.Name,
                        Latitude = f.FacilityLocation.Latitude,
                        Longitude = f.FacilityLocation.Longitude
                    },

                    FacilityOperatingHours = f.FacilityOperatingHours,

                    FacilityAmenities = f.FacilityAmenities
                        .Select(a => new EditFacilityAmenityDto
                        {
                            AmenityId = a.AmenityID,
                            AmenityName = a.Amenity.name
                        }).ToList(),

                    FacilityServices = f.FacilityServices
                        .Select(s => new EditFacilityServiceDto
                        {
                            ServiceId = s.CoreFacilityServiceID,
                            ServiceName = s.CoreFacilityServices.Name
                        }).ToList(),

                    FacilityAccessibilityOptions = f.FacilityAccessibilityOptions
                        .Select(ac => new EditFacilityAccessibilityOptionDto
                        {
                            AccessibilityOptionId = ac.AccessibilityOptionID,
                            AccessibilityOptionName = ac.AccessibilityOptions.OptionName
                        }).ToList(),

                    SportsList = f.SportsList
                        .Select(sp => new EditFacilitySportDto
                        {
                            SportId = sp.SportID,
                            SportName = sp.Sports.SportName
                        }).ToList(),

                    FacilityUploads = f.FacilityUploads
                        .Select(img => new EditFacilityImageDto
                        {
                            Id = img.Id,
                            ImageUrl = img.ImageUrl
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return editFacilityDto;
        }

        public async Task<List<MobileFacilityVenuesDto>> GetMobileFacilityVenuesAsync(int facilityID)
        {
            var result = await _context.NewVenues
                .Where(v => v.FacilityId == facilityID)
                .Select(v => new MobileFacilityVenuesDto
                {
                    Id = v.Id,
                    ImageURL = v.VenueImageURL,
                    VenueName = v.Name,
                    Rating = v.VenueReviews.Any()
                            ? Math.Round(v.VenueReviews.Average(r => r.Rating), 1) : 0
                })
                .ToListAsync();

            return result;
        }
        public async Task<GetFacilityMobileDetailsDto> GetMobileFacilityDetailsAsync(int facilityID)
        {
            var facilityOperatingHours = await _context.NewFacilityOperatingHours
                .FirstOrDefaultAsync(f => f.FacilityId == facilityID);

            var facilityMobileDetails = await _context.NewFacility
                .Where(f => f.Id == facilityID)
                .Include(f => f.FacilityUploads)
                .Select(f => new GetFacilityMobileDetailsDto
                {
                    FacilityHeader = new FacilityHeaderMobile
                    {
                        FacilityImageUrl = f.FacilityImageUrl,
                        FacilityName = f.FacilityName,
                        BranchName = f.Branch.Name
                    },
                    FacilityAbout = new FacilityAboutMobile
                    {
                        BranchImage = f.Branch.BranchImageURL,
                        BranchName = f.Branch.Name,
                        PhoneNumber = $"{f.CountryCodeID} {f.PhoneNumber}",
                        EmailAddress = f.Branch.EmailAddress,
                        Country = _context.Country
                            .Where(c => c.Id == f.Branch.CountryID)
                            .Select(c => c.Name)
                            .FirstOrDefault(),
                        Governorate = _context.State
                            .Where(s => s.Id == f.Branch.StateID)
                            .Select(s => s.Name)
                            .FirstOrDefault()
                    },
                    Sports = f.SportsList
                        .Select(s => s.Sports.SportName)
                        .ToList(),
                    Accessibility = f.FacilityAccessibilityOptions
                        .Select(a => a.AccessibilityOptions.OptionName)
                        .ToList(),
                    Amenities = f.FacilityAmenities
                        .Select(a => a.Amenity.name)
                        .ToList(),
                    OperatingHours = facilityOperatingHours,
                    ImageUrls = f.FacilityUploads.Select(m => m.ImageUrl).ToList()
                })
                .FirstOrDefaultAsync();

            return facilityMobileDetails;
        }
        public async Task<List<NewFacilityDashboardDto>> GetFaciliesByBranchID(int branchID)
        {

            var facilityList = await _context.NewFacility
                .Where(f => f.BranchId == branchID)
                .Select(f => new NewFacilityDashboardDto
                {
                  Id = f.Id,
                  name = f.FacilityName
                })
                .ToListAsync();

            return facilityList;
        }

    }
}
