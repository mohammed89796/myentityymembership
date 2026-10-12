using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.VenueBookingTypes
{
    public interface IVenueBookingTypesService
    {
        Task<Response<bool>> CreateVenueBookingTypes(string name, string description, IFormFile Icon);
        Task<Response<List<Data.Entity.Entities.VenueBookingTypes>>> GetVenueBookingTypesAsync();

    }
}
