namespace MyEntityMembershipService.DTOs.Entity.VenueDtos;

public class VenueBookingQueryParametersDto
{
    public int VenueId { get; set; }
    public int? VenueType { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageIndex { get; set; } = 1;
}