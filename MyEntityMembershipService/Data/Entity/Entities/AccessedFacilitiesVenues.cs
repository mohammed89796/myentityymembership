using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class AccessedFacilitiesVenues
    {
        [Key]

        public int PackageId { get; set; }
        public Package Package { get; set; }
        [Key]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Key]

        public int FacilityId { get; set; }
        public NewFacility Facility { get; set; }


        [Key]

        public int VenueId { get; set; }
        public NewVenue Venue { get; set; }

    }
}
