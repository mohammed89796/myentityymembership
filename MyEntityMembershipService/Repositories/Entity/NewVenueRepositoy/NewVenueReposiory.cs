using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.NewVenueRepositoy
{
    public class NewVenueReposiory : GenericRepository<NewVenue>, INewVenueRepository
    {
        private readonly EntityMembershipDbContext _myEntityDbContext;

        public NewVenueReposiory(EntityMembershipDbContext myEntityDbContext) : base(myEntityDbContext)
        {
            _myEntityDbContext = myEntityDbContext;
        }


         public async Task<GetVenuePaginationListDto> GetVenuesPaginatedAsync(int pageNumber, int facilityId)
         {
             int pageSize = 8;
             var totalVenues = await _myEntityDbContext.NewVenue
                 .Where(v => v.FacilityId == facilityId)
                 .CountAsync();

             var totalPages = (int)Math.Ceiling((double)totalVenues / pageSize);

             var venues = await _myEntityDbContext.NewVenue
                 .Where(v => v.FacilityId == facilityId)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .Select(v => new VenueCardDetailsDto
                 {
                     VenueID = v.Id,
                     ImageURL = v.VenueImageURL,
                     VenueName = v.Name,
                     VenueType = v.VenueType.VenueTypes.TypeName,
                     Rating = v.VenueReviews.Any()
                         ? Math.Round(v.VenueReviews.Average(r => r.Rating), 2)
                         : 0
                 })
                 .ToListAsync();

             return new GetVenuePaginationListDto
             {
                 TotalPages = totalPages,
                 CurrentPage = pageNumber,
                 Venues = venues
             };
         }
        
        //public async Task<GetVenuePaginationListDto> GetVenuesPaginatedAsync(int pageNumber, int facilityId)
        //{
        //    int pageSize = 8;
        //    var totalVenues = await _myEntityDbContext.NewVenue
        //        .Where(v => v.FacilityId == facilityId)
        //        .Join(_myEntityDbContext.NewVenueBookingConfiguration,
        //            venue => venue.Id,
        //            config => config.VenueID,
        //            (venue, config) => new { Venue = venue, Config = config })
        //        // .Where(x => x.Config.IsVisible)
        //        .CountAsync();

        //    var totalPages = (int)Math.Ceiling((double)totalVenues / pageSize);

        //    var venues = await _myEntityDbContext.NewVenue
        //        .Where(v => v.FacilityId == facilityId)
        //        .Join(_myEntityDbContext.NewVenueBookingConfiguration,
        //            venue => venue.Id,
        //            config => config.VenueID,
        //            (venue, config) => new { Venue = venue, Config = config })
        //        // .Where(x => x.Config.IsVisible)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(x => new VenueCardDetailsDto
        //        {
        //            VenueID = x.Venue.Id,
        //            ImageURL = x.Venue.VenueImageURL,
        //            VenueName = x.Venue.Name,
        //            VenueType = x.Venue.VenueType.VenueTypes.TypeName,
        //            Rating = x.Venue.VenueReviews.Any()
        //                ? Math.Round(x.Venue.VenueReviews.Average(r => r.Rating), 2)
        //                : 0
        //        })
        //        .ToListAsync();

        //    return new GetVenuePaginationListDto
        //    {
        //        TotalPages = totalPages,
        //        CurrentPage = pageNumber,
        //        Venues = venues
        //    };
        //}
       
        public async Task<GetVenuePaginationListDto> GetMobileVenuesPaginatedAsync(int pageNumber, int facilityId)
        {
            int pageSize = 8;
            var totalVenues = await _myEntityDbContext.NewVenue
                .Where(v => v.FacilityId == facilityId)
                .Join(_myEntityDbContext.NewVenueBookingConfiguration,
                    venue => venue.Id,
                    config => config.VenueID,
                    (venue, config) => new { Venue = venue, Config = config })
                // .Where(x => x.Config.IsVisible)
                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalVenues / pageSize);

            var venues = await _myEntityDbContext.NewVenue
                .Where(v => v.FacilityId == facilityId)
                .Join(_myEntityDbContext.NewVenueBookingConfiguration,
                    venue => venue.Id,
                    config => config.VenueID,
                    (venue, config) => new { Venue = venue, Config = config })
                .Where(x => x.Config.IsVisible)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new VenueCardDetailsDto
                {
                    VenueID = x.Venue.Id,
                    ImageURL = x.Venue.VenueImageURL,
                    VenueName = x.Venue.Name,
                    VenueType = x.Venue.VenueType.VenueTypes.TypeName,
                    Rating = x.Venue.VenueReviews.Any()
                        ? Math.Round(x.Venue.VenueReviews.Average(r => r.Rating), 2)
                        : 0
                })
                .ToListAsync();

            return new GetVenuePaginationListDto
            {
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                Venues = venues
            };
        }
       
        public async Task<GetVenueDetailsDto> GetVenueDetailsAsync(int venueId)
        {
            var venue = await _myEntityDbContext.NewVenue
                .Where(v => v.Id == venueId)
                .Include(v => v.OperatingHours)
                .Select(v => new GetVenueDetailsDto
                {
                    BranchID = v.Facility.Branch.Id,
                    FacilityName = v.Facility.FacilityName,
                    HeaderDetails = new VenueHeaderDetailsDto
                    {
                        VenueName = v.Name,
                        VanueImageURL = v.VenueImageURL,
                        FacilityName = v.Facility.FacilityName,
                        CountryCode = v.Facility.CountryCodeID,
                        PhoneNumber = $"{v.Facility.CountryCodeID} {v.Facility.PhoneNumber}",
                        BranchName = v.Facility.Branch.Name,
                        BranchImageURL = v.Facility.Branch.BranchImageURL,
                        Email = v.Facility.Branch.EmailAddress,
                        Address = v.Facility.Branch.Location,
                        Country = string.Concat(
                            _myEntityDbContext.Country
                                .Where(c => c.Id == v.Facility.Branch.CountryID)
                                .Select(c => c.Name)
                                .FirstOrDefault() ?? "Unknown Country", ", ",
                            _myEntityDbContext.State
                                .Where(s => s.Id == v.Facility.Branch.StateID)
                                .Select(s => s.Name)
                                .FirstOrDefault() ?? "Unknown State"),
                    },

                    FeaturesDetails = new VenueFeaturesDetailsDto
                    {
                        VenueCapacity = v.VenueCapacity,
                        Dimensions = v.FieldDimensions,
                        Type = v.VenueType.VenueTypes.TypeName,
                        TrufType = v.TurfTypes.FirstOrDefault().VenueTurfTypes.TypeName,
                        Utilities = v.VenueUtilities.Select(ut => ut.VenueUtilities.UtilityName).ToList(),
                        Accessibility = v.VenueAccessibilityOptions.Select(a => a.AccessibilityOptions.OptionName).ToList(),
                        Amenities = v.VenueAmenities.Select(a => a.Amenity.name).ToList()
                    },

                    LocationDetails = new VenueLocationDetailsDto
                    {
                        Name = v.VenueLocation.Name,
                        Latitude = v.VenueLocation.Latitude,
                        Longitude = v.VenueLocation.Longitude
                    },

                    VenueImages = v.VenueUploads.ToList()
                })
                .FirstOrDefaultAsync();

            var operatingHours = await _myEntityDbContext.VenueOperatingHours
                .Where(vo => vo.NewVenueId == venueId)
                .FirstOrDefaultAsync();

            venue.OperatingHours = operatingHours != null ? new VenueOperatingHoursDetailsDto
            {
                SundayOpenAt = operatingHours.sundayOpenAt,
                SundayCloseAt = operatingHours.sundayCloseAt,
                SundayIsWorking = operatingHours.sundayIsWorking,
                MondayOpenAt = operatingHours.mondayOpenAt,
                MondayCloseAt = operatingHours.mondayCloseAt,
                MondayIsWorking = operatingHours.mondayIsWorking,
                TuesdayOpenAt = operatingHours.tuesdayOpenAt,
                TuesdayCloseAt = operatingHours.tuesdayCloseAt,
                TuesdayIsWorking = operatingHours.tuesdayIsWorking,
                WednesdayOpenAt = operatingHours.wednesdayOpenAt,
                WednesdayCloseAt = operatingHours.wednesdayCloseAt,
                WednesdayIsWorking = operatingHours.wednesdayIsWorking,
                ThursdayOpenAt = operatingHours.thursdayOpenAt,
                ThursdayCloseAt = operatingHours.thursdayCloseAt,
                ThursdayIsWorking = operatingHours.thursdayIsWorking,
                FridayOpenAt = operatingHours.fridayOpenAt,
                FridayCloseAt = operatingHours.fridayCloseAt,
                FridayIsWorking = operatingHours.fridayIsWorking,
                SaturdayOpenAt = operatingHours.saturdayOpenAt,
                SaturdayCloseAt = operatingHours.saturdayCloseAt,
                SaturdayIsWorking = operatingHours.saturdayIsWorking
            } : null;

            return venue;
        }

        public async Task<List<GetVenueDto>> GetVenuesByListOfIdsAsync(List<int> venuesIds)

           => await _myEntityDbContext.NewVenue.Where(v => venuesIds.Contains(v.Id)).Select(v =>new GetVenueDto 
           {
               ID = v.Id,
              Name= v.Name,
               Image = v.VenueImageURL,
               VenueLocation = v.VenueLocation.Name,
               VenueLongitude = v.VenueLocation.Longitude,
               VenueLatitude = v.VenueLocation.Latitude,

               Facility = v.Facility.FacilityName,
               FacilityImage = v.Facility.FacilityImageUrl,
               FacilityLocation = v.Facility.FacilityLocation.Name,
               FacilityLongitude = v.Facility.FacilityLocation.Longitude,
               FacilityLatitude = v.Facility.FacilityLocation.Latitude,

               Branch = v.Facility.Branch.Name,
               BranchImage = v.Facility.Branch.BranchImageURL,
               BranchLocation = v.Facility.Branch.BranchLocation.Name,
               BranchLongitude = v.Facility.Branch.BranchLocation.Longitude,
               BranchLatitude = v.Facility.Branch.BranchLocation.Latitude
           }).ToListAsync();

        public async Task<List<GetVenueWrapperDto>> GetVenuesInfoByListOfIdsAsync(List<int> venuesIds)

           => await _myEntityDbContext.NewVenue.Where(v => venuesIds.Contains(v.Id)).Select(v => new GetVenueWrapperDto
           {

               Venue = new VenueWrapperDto
               {
                 
                  Id= v.Id,
                  Name= v.Name,
                   Image = v.VenueImageURL,
                   VenueLocation = v.VenueLocation.Name,
                   VenueLongitude = v.VenueLocation.Longitude,
                   VenueLatitude = v.VenueLocation.Latitude,



                   Facility = new DTOs.Entity.FacilityDtos.FacilityWrapperDto
                   {
                      Id= v.Facility.Id,
                       Name = v.Facility.FacilityName,
                       Image = v.Facility.FacilityImageUrl,
                       FacilityLocation = v.Facility.FacilityLocation.Name,
                       FacilityLatitude = v.Facility.FacilityLocation.Latitude,
                       FacilityLongitude = v.Facility.FacilityLocation.Longitude
                   },
                   Branch = new DTOs.Entity.BranchDtos.BranchWrapperDto
                   {
                      Id= v.Facility.Branch.Id,
                      Name= v.Facility.Branch.Name,
                       Image = v.Facility.Branch.BranchImageURL,
                       BranchLocation = v.Facility.Branch.BranchLocation.Name,
                       BranchLatitude = v.Facility.Branch.BranchLocation.Latitude,
                       BranchLongitude = v.Facility.Branch.BranchLocation.Longitude

                   }
               },


           }).ToListAsync();

        public async Task<List<NewVenue>> GetVenuesByNameAsync(string venueName)
        {
            return await _myEntityDbContext.NewVenue.Where(x => x.Name.ToLower().Contains(venueName.ToLower())).ToListAsync();
        }

        public async Task<List<VenueBookingDto>> GetVenueBookingsAsync(GetVenueBookingsDto request)
        {
            DateTime normalizedStartDate = request.StartDate.Date;
            DateTime endDate;

            switch (request.View.ToLower())
            {
                case "day":
                    endDate = normalizedStartDate.AddDays(1);
                    break;
                case "week":
                    normalizedStartDate = StartOfWeek(normalizedStartDate, DayOfWeek.Sunday);
                    endDate = normalizedStartDate.AddDays(7);
                    break;
                case "month":
                    normalizedStartDate = StartOfMonth(normalizedStartDate);
                    endDate = EndOfMonth(normalizedStartDate);
                    break;
                default:
                    throw new ArgumentException("Invalid view type");
            }

            const int pageSize = 2;
            int skip = (request.PageNumber - 1) * pageSize;

            int totalVenuesCount = await _myEntityDbContext.NewVenue
                .Where(v => v.FacilityId == request.FacilityId)
                .CountAsync();

            var venueList = await _myEntityDbContext.NewVenue
                .Where(v => v.FacilityId == request.FacilityId)
                .OrderBy(v => v.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var venueBookings = new List<VenueBookingDto>();

            foreach (var venue in venueList)
            {
                // Retrieve bookings for the venue
                var bookings = await _myEntityDbContext.VenueBooking
                    .Where(b => b.VenueId == venue.Id && b.BookingDate >= normalizedStartDate && b.BookingDate < endDate)
                    .Include(b => b.BookingSlots)
                    .ToListAsync();

                // Map bookings into CalendarEventDto
                var calendarEvents = bookings
                    .Select(b => new CalendarEventDto
                    {
                        Id = b.Id,
                        Title = b.BookingType switch
                        {
                            1 => "Event",
                            2 => "Out Of Service",
                            3 => "Member Booking",
                            _ => "Booking" // Default
                        },
                        BookingDate = b.BookingDate,
                        Start = b.BookingSlots.Min(slot => slot.SlotStart),
                        End = b.BookingSlots.Max(slot => slot.SlotEnd)
                    })
                    .OrderBy(e => e.BookingDate)
                    .ThenBy(e => e.Start)
                    .ToList();

                var venueConfigurationEntity = await _myEntityDbContext.NewVenueBookingConfiguration
                    .FirstOrDefaultAsync(c => c.VenueID == venue.Id);

                var venueConfiguration = venueConfigurationEntity == null ? null : new NewVenueBookingConfigurationDto
                {
                    SlotTime = venueConfigurationEntity.SlotTime,
                    PricePerSlot = venueConfigurationEntity.PricePerSlot,
                    MinSlotsBooked = venueConfigurationEntity.MinSlotsBooked,
                    MaxSlotsBooked = venueConfigurationEntity.MaxSlotsBooked,
                    CancellationTime = venueConfigurationEntity.CancellationTime,
                    BookingAvailability = venueConfigurationEntity.BookingAvailability,
                    BookingAvailabilityUnit = venueConfigurationEntity.BookingAvailabilityUnit,
                    IsVisible = venueConfigurationEntity.IsVisible,
                    AllowBooking = venueConfigurationEntity.AllowBooking
                };

                var venueOperatingHoursEntity = await _myEntityDbContext.VenueOperatingHours
                    .FirstOrDefaultAsync(o => o.NewVenueId == venue.Id);

                var venueOperatingHours = venueOperatingHoursEntity == null ? null : new VenueOperatingHoursDto
                {
                    VenueID = venueOperatingHoursEntity.NewVenueId,
                    SundayOpenAt = venueOperatingHoursEntity.sundayOpenAt?.ToString("hh\\:mm"),
                    SundayCloseAt = venueOperatingHoursEntity.sundayCloseAt?.ToString("hh\\:mm"),
                    SundayIsWorking = venueOperatingHoursEntity.sundayIsWorking,
                    MondayOpenAt = venueOperatingHoursEntity.mondayOpenAt?.ToString("hh\\:mm"),
                    MondayCloseAt = venueOperatingHoursEntity.mondayCloseAt?.ToString("hh\\:mm"),
                    MondayIsWorking = venueOperatingHoursEntity.mondayIsWorking,
                    TuesdayOpenAt = venueOperatingHoursEntity.tuesdayOpenAt?.ToString("hh\\:mm"),
                    TuesdayCloseAt = venueOperatingHoursEntity.tuesdayCloseAt?.ToString("hh\\:mm"),
                    TuesdayIsWorking = venueOperatingHoursEntity.tuesdayIsWorking,
                    WednesdayOpenAt = venueOperatingHoursEntity.wednesdayOpenAt?.ToString("hh\\:mm"),
                    WednesdayCloseAt = venueOperatingHoursEntity.wednesdayCloseAt?.ToString("hh\\:mm"),
                    WednesdayIsWorking = venueOperatingHoursEntity.wednesdayIsWorking,
                    ThursdayOpenAt = venueOperatingHoursEntity.thursdayOpenAt?.ToString("hh\\:mm"),
                    ThursdayCloseAt = venueOperatingHoursEntity.thursdayCloseAt?.ToString("hh\\:mm"),
                    ThursdayIsWorking = venueOperatingHoursEntity.thursdayIsWorking,
                    FridayOpenAt = venueOperatingHoursEntity.fridayOpenAt?.ToString("hh\\:mm"),
                    FridayCloseAt = venueOperatingHoursEntity.fridayCloseAt?.ToString("hh\\:mm"),
                    FridayIsWorking = venueOperatingHoursEntity.fridayIsWorking,
                    SaturdayOpenAt = venueOperatingHoursEntity.saturdayOpenAt?.ToString("hh\\:mm"),
                    SaturdayCloseAt = venueOperatingHoursEntity.saturdayCloseAt?.ToString("hh\\:mm"),
                    SaturdayIsWorking = venueOperatingHoursEntity.saturdayIsWorking
                };

                venueBookings.Add(new VenueBookingDto
                {
                    Id = venue.Id,
                    TotalNumberOfVenues = totalVenuesCount,
                    VenueId = venue.Id,
                    VenueName = venue.Name,
                    VenueConfiguration = venueConfiguration,
                    VenueOperatingHours = venueOperatingHours,
                    Bookings = calendarEvents
                });
            }

            return venueBookings;
        }

        private static DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek)
        {
            int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        private static DateTime StartOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        private static DateTime EndOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)).AddDays(1).AddTicks(-1);
        }

        public async Task<GetVenueBookingsMobileDto> GetVenuesBookingsMobile(int venueID, string userId)
        {
            var venue = await _myEntityDbContext.NewVenues
                .Include(v => v.VenueReviews)
                .Include(v => v.Facility)
                .FirstOrDefaultAsync(v => v.Id == venueID);

            if (venue == null)
            {
                throw new KeyNotFoundException($"Venue with ID {venueID} not found.");
            }

            var venueRating = venue.VenueReviews.Any()
                ? (int)Math.Round(venue.VenueReviews.Average(r => r.Rating))
                : 0;

            var bookedSlots = await _myEntityDbContext.VenueBooking
                .Where(vb => vb.VenueId == venueID && vb.MemberID == userId)
                .Select(vb => new GetVenueBookedSlotsMobileDto
                {
                    BookingID = vb.Id,
                    BookingDate = vb.BookingDate,
                    VenueName = vb.Venue.Name,
                    SlotStartTime = vb.BookingSlots.Min(bs => bs.SlotStart),
                    SlotEndTime = vb.BookingSlots.Max(bs => bs.SlotEnd),
                    Address = vb.Venue.Facility.Branch.Location,
                    FacilityName = $"{vb.Venue.Facility.FacilityName}, {vb.Venue.Facility.Branch.Name}"
                })
                .ToListAsync();

            return new GetVenueBookingsMobileDto
            {
                VenueID = venue.Id,
                VenueImage = venue.VenueImageURL,
                VenueName = venue.Name,
                VenueRating = venueRating,
                BookedSlots = bookedSlots
            };
        }


        public async Task<string> GetCancellationPolicyMessage(int venueID)
        {
            var cancellationTime = await _myEntityDbContext.NewVenueBookingConfiguration
                .Where(config => config.VenueID == venueID)
                .Select(config => config.CancellationTime)
                .FirstOrDefaultAsync();

            if (cancellationTime == 0)
            {
                throw new KeyNotFoundException($"Cancellation policy not found for venue with ID {venueID}.");
            }

            return $"Cancellation is allowed up to {cancellationTime} hours prior to the booking time.";
        }

    }
}
