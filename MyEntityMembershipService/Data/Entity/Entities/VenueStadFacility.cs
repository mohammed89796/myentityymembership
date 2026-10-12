namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class VenueStadFacility
    {
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        public int StadFacilityId { get; set; }
        public StadFacilities? StadFacility { get; set; }
    }
}
