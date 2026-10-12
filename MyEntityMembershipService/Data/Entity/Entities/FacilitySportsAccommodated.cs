namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class FacilitySportsAccommodated
    {
        public int FacilityId { get; set; }
        public Facility? Facility { get; set; }

        public int SportsAccommodatedId { get; set; }
        public SportsAccommodated? SportsAccommodated { get; set; }
    }
}
