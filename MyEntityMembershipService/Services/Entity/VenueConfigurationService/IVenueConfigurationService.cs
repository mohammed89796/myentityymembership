
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.VenueConfigurationService
{
    public interface IVenueConfigurationService
    {
        Task<Response<NewVenueBookingConfiguration>> CreateNewVenueConfigurationAsync(AddVenueConfigurationDto request);
        Task<Response<GetVenueConfiguration>> GetVenueConfiguration(int venueID);
    }
}
