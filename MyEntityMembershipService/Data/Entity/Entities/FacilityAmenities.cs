namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class FacilityAmenities
    {
        public int FacilityId { get; set; }
        public Facility? Facility { get; set; }

        public int StadFacilityId { get; set; }
        public StadFacilities? StadFacility { get; set; }
    }
}
