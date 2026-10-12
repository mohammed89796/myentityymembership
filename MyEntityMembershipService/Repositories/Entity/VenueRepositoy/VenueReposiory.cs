using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.VenueRepositoy
{
    public class VenueReposiory : GenericRepository<Venue>, IVenueRepository
    {
        private readonly EntityMembershipDbContext _myEntityDbContext;

        public VenueReposiory(EntityMembershipDbContext myEntityDbContext) : base(myEntityDbContext)
        {
            _myEntityDbContext = myEntityDbContext;
        }

        public async Task<List<object>> GetVenuesByListOfIdsAsync(List<int> venuesIds)

            => await _myEntityDbContext.Venues.Where(v => venuesIds.Contains(v.Id)).Select(v => (object)new { ID = v.Id, Name = v.name, v.Image, v.venueLocation, Branch = v.Facility.Branch.Name, BranchLocation = v.Facility.Branch.Location }).ToListAsync();


        public async Task<List<object>> GetVenuesInfoByListOfIdsAsync(List<int> venuesIds)

            => await _myEntityDbContext.Venues.Where(v => venuesIds.Contains(v.Id)).Select(v => (object)new
            {

                Venue = new
                {
                    v.Id,
                    Name = v.name,
                    v.Image,
                    v.venueLocation,
                    Facility = new
                    {
                        v.Facility.Id,
                        Name = v.Facility.name,
                        v.Facility.Image,
                        Address = v.Facility.address,
                        City = v.Facility.city,
                        Country = v.Facility.country,
                        Latitude = v.Facility.latitude,
                        Longitude = v.Facility.longitude
                    },
                    Branch = new
                    {
                        v.Facility.Branch.Id,
                        v.Facility.Branch.Name,
                        Image = v.Facility.Branch.BranchImageURL,
                        v.Facility.Branch.Location
                    }
                },
            }).ToListAsync();

        public async Task<List<Venue>> GetVenuesByNameAsync(string venueName)
        {
            return await _myEntityDbContext.Venues.Where(x => x.name.ToLower().Contains(venueName.ToLower())).ToListAsync();
        }
        public async Task<EditVenueDetailsDto> GetVenueEditDetailsByIdAsync(int venueID)
        {
            var editVenueDto = await _myEntityDbContext.NewVenues
                .Include(v => v.OperatingHours)
                .Include(v => v.VenueLocation)
                .Include(v => v.VenueUtilities)
                .Include(v => v.VenueAccessibilityOptions)
                .Include(v => v.VenueAmenities)
                .Include(v => v.TurfTypes)
                .Include(v => v.VenueUploads)
                .Where(v => v.Id == venueID)
                .Select(v => new EditVenueDetailsDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    VenueTypeId = v.VenueType.Id,
                    VenueTypeName = v.VenueType.VenueTypes.TypeName,
                    VenueCapacity = v.VenueCapacity,
                    Description = v.Description,
                    FieldDimensions = v.FieldDimensions,
                    FacilityId = v.FacilityId,
                    VenueImageURL = v.VenueImageURL,

                    VenueLocation = new EditVenueLocationDto
                    {
                        Name = v.VenueLocation.Name,
                        Latitude = v.VenueLocation.Latitude,
                        Longitude = v.VenueLocation.Longitude
                    },


                    //FIXME: Operating hours is not returning while fetching the details.
                    OperatingHours = v.OperatingHours != null ? new EditVenueOperatingHoursDetailsDto
                    {
                        SundayOpenAt = v.OperatingHours.sundayOpenAt,
                        SundayCloseAt = v.OperatingHours.sundayCloseAt,
                        SundayIsWorking = v.OperatingHours.sundayIsWorking,

                        MondayOpenAt = v.OperatingHours.mondayOpenAt,
                        MondayCloseAt = v.OperatingHours.mondayCloseAt,
                        MondayIsWorking = v.OperatingHours.mondayIsWorking,

                        TuesdayOpenAt = v.OperatingHours.tuesdayOpenAt,
                        TuesdayCloseAt = v.OperatingHours.tuesdayCloseAt,
                        TuesdayIsWorking = v.OperatingHours.tuesdayIsWorking,

                        WednesdayOpenAt = v.OperatingHours.wednesdayOpenAt,
                        WednesdayCloseAt = v.OperatingHours.wednesdayCloseAt,
                        WednesdayIsWorking = v.OperatingHours.wednesdayIsWorking,

                        ThursdayOpenAt = v.OperatingHours.thursdayOpenAt,
                        ThursdayCloseAt = v.OperatingHours.thursdayCloseAt,
                        ThursdayIsWorking = v.OperatingHours.thursdayIsWorking,

                        FridayOpenAt = v.OperatingHours.fridayOpenAt,
                        FridayCloseAt = v.OperatingHours.fridayCloseAt,
                        FridayIsWorking = v.OperatingHours.fridayIsWorking,

                        SaturdayOpenAt = v.OperatingHours.saturdayOpenAt,
                        SaturdayCloseAt = v.OperatingHours.saturdayCloseAt,
                        SaturdayIsWorking = v.OperatingHours.saturdayIsWorking,
                    } : null,


                    VenueUtilities = v.VenueUtilities
                        .Select(util => new EditVenueUtilityDto
                        {
                            UtilityId = util.UtilityID,
                            UtilityName = util.VenueUtilities.UtilityName
                        }).ToList(),

                    VenueAccessibilityOptions = v.VenueAccessibilityOptions
                        .Select(opt => new EditVenueAccessibilityOptionDto
                        {
                            AccessibilityOptionId = opt.AccessibilityOptionID,
                            AccessibilityOptionName = opt.AccessibilityOptions.OptionName
                        }).ToList(),

                    VenueAmenities = v.VenueAmenities
                        .Select(am => new EditVenueAmenityDto
                        {
                            AmenityId = am.AmenityID,
                            AmenityName = am.Amenity.name
                        }).ToList(),

                    TurfTypes = v.TurfTypes
                        .Select(tt => new EditVenueTurfTypeDto
                        {
                            TurfTypeId = tt.TurfTypeID,
                            TurfTypeName = tt.VenueTurfTypes.TypeName
                        }).ToList(),

                    VenueUploads = v.VenueUploads
                        .Select(img => new EditVenueImageDto
                        {
                            Id = img.Id,
                            ImageUrl = img.ImageUrl
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return editVenueDto;
        }

        public async Task<GetVenueMobileDetailsDto> GetMobileVenueDetailsAsync(int venueID)
        {
            var operatingHours = await _myEntityDbContext.VenueOperatingHours
                .Where(v => v.NewVenueId == venueID && !v.IsDeleted)
                .FirstOrDefaultAsync();

            var venueDetails = await _myEntityDbContext.NewVenues
                .Include(v => v.OperatingHours)
                .Where(v => v.Id == venueID)
                .Select(v => new GetVenueMobileDetailsDto
                {
                    BranchID = v.Facility.Branch.Id,
                    FacilityID = v.FacilityId,
                    FacilityName = v.Facility.FacilityName,
                    VenueImageURL = v.VenueImageURL,
                    VenueName = v.Name,
                    Rating = _myEntityDbContext.VenueReviews
                            .Where(r => r.VenueID == venueID)
                            .Select(r => (double?)r.Rating)
                            .DefaultIfEmpty()
                            .Average() ?? 0,
                    VenueAbout = new GetVenueAboutDetailsDto
                    {
                        PhoneNumber = $"{v.Facility.CountryCodeID} {v.Facility.PhoneNumber}",
                        Email = v.Facility.Branch.EmailAddress,
                        Country = _myEntityDbContext.Country
                            .Where(c => c.Id == v.Facility.Branch.CountryID)
                            .Select(c => c.Name)
                            .FirstOrDefault(),
                        Governnorate = _myEntityDbContext.State
                            .Where(c => c.Id == v.Facility.Branch.StateID)
                            .Select(c => c.Name)
                            .FirstOrDefault(),
                    },
                    VenueFeatures = new GetVenueFeaturesDto
                    {
                        VenueCapacity = v.VenueCapacity,
                        Dimensions = v.FieldDimensions,
                        Type = null,
                        TurfType = v.TurfTypes.Select(tt => tt.VenueTurfTypes.TypeName).FirstOrDefault(),
                        Utilities = v.VenueUtilities.Select(u => u.VenueUtilities.UtilityName).ToList(),
                        Accessibility = v.VenueAccessibilityOptions.Select(a => a.AccessibilityOptions.OptionName).ToList(),
                        Amenities = v.VenueAmenities.Select(a => a.Amenity.name).ToList(),
                    },
                    VenueMedia = v.VenueUploads.Select(m => new GetVenueMediaDto
                    {
                        ID = m.Id,
                        ImageUrl = m.ImageUrl
                    }).ToList(),
                    OperatingHours = operatingHours
                })
                .FirstOrDefaultAsync();

            return venueDetails;
        }

        public async Task<GetVenueBookingHeaderDetailsDto> GetVenueBookingHeaderDetailsAsync(int venueId)
        {
            var venueDetails = await _myEntityDbContext.NewVenues
                .Where(v => v.Id == venueId)
                .Select(v => new GetVenueBookingHeaderDetailsDto
                {
                    VenueImageURL = v.VenueImageURL,
                    VenueName = v.Name,
                    BranchName = v.Facility.Branch.Name,
                    Email = v.Facility.Branch.EmailAddress,
                    Address = v.Facility.Branch.Location,
                    State = $"{_myEntityDbContext.State
                            .Where(c => c.Id == v.Facility.Branch.StateID)
                            .Select(c => c.Name)
                            .FirstOrDefault()} {_myEntityDbContext.State
                            .Where(s => s.Id == v.Facility.Branch.StateID)
                            .Select(s => s.Name)
                            .FirstOrDefault()}",

                    PhoneNumber = $"{v.Facility.CountryCodeID} {v.Facility.PhoneNumber}",
                    Features = new GetVenueBookingFEaturesDetailsDto
                    {
                        Capacity = v.VenueCapacity.ToString(),
                        Dimensions = v.FieldDimensions,
                        Type = v.VenueType.ToString(), // Adjust based on how VenueType is stored
                        TurfTypes = v.TurfTypes.Select(t => t.VenueTurfTypes.TypeName).FirstOrDefault(),
                        Utilities = v.VenueUtilities.Select(u => u.VenueUtilities.UtilityName).ToList(),
                        Accessibility = v.VenueAccessibilityOptions.Select(a => a.AccessibilityOptions.OptionName).ToList(),
                        Amenities = v.VenueAmenities.Select(a => a.Amenity.name).ToList()
                    }
                })
                .FirstOrDefaultAsync();

            return venueDetails;
        }
        public async Task<GetRescheduleMemberDetails> GetRescheduleMemberDetails(int bookingID)
        {
            var rescheduleDetails = await _myEntityDbContext.VenueBooking
                .Where(vb => vb.Id == bookingID)
                .Select(vb => new GetRescheduleMemberDetails
                {
                    BookingID = vb.Id,
                    BookingDate = vb.BookingDate.ToString("yyyy-MM-dd"),
                    StartTime = vb.BookingSlots.Min(bs => bs.SlotStart),
                    EndTime = vb.BookingSlots.Max(bs => bs.SlotEnd),
                    MemberName = vb.MemberName,
                    MemberID = vb.MemberID,
                    PhoneNumber = vb.PhoneNumber,
                    VenueName = vb.Venue.Name,
                    VenueImage = vb.Venue.VenueImageURL,
                    BranchName = vb.Venue.Facility.Branch.Name,
                    PricePaided = vb.Price.HasValue ? (double)vb.Price.Value : 0.0
                })
                .FirstOrDefaultAsync();

            return rescheduleDetails;
        }
        public async Task<GetRescheduleOutOfServiceDetails> GetRescheduleOutOfServiceBookingDetails(int bookingID)
        {
            var rescheduleDetails = await _myEntityDbContext.VenueBooking
                .Where(vb => vb.Id == bookingID)
                .Select(vb => new GetRescheduleOutOfServiceDetails
                {
                    BookingID = vb.Id,
                    BookingDate = vb.BookingDate.ToString("yyyy-MM-dd"),
                    StartTime = vb.BookingSlots.Min(bs => bs.SlotStart),
                    EndTime = vb.BookingSlots.Max(bs => bs.SlotEnd),
                    VenueName = vb.Venue.Name,
                    VenueImage = vb.Venue.VenueImageURL,
                    BranchName = vb.Venue.Facility.Branch.Name
                })
                .FirstOrDefaultAsync();

            return rescheduleDetails;
        }
        public async Task<GetRescheduleEventDetails> GetRescheduleEventBookingDetails(int bookingID)
        {
            var rescheduleDetails = await _myEntityDbContext.VenueBooking
                .Where(vb => vb.Id == bookingID)
                .Select(vb => new GetRescheduleEventDetails
                {
                    BookingID = vb.Id,
                    BookingDate = vb.BookingDate.ToString("yyyy-MM-dd"),
                    StartTime = vb.BookingSlots.Min(bs => bs.SlotStart),
                    EndTime = vb.BookingSlots.Max(bs => bs.SlotEnd),
                    EventName = vb.EventName,
                    EventType = vb.EventypeID.ToString(),
                    Description = vb.Description,
                    IsNoneMember = vb.IsNonMember,
                    VenueName = vb.Venue.Name,
                    VenueImage = vb.Venue.VenueImageURL,
                    BranchName = vb.Venue.Facility.Branch.Name
                })
                .FirstOrDefaultAsync();

            return rescheduleDetails;
        }
    }

}
