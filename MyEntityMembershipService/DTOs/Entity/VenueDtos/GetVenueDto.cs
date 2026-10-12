namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetVenueDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public string VenueLocation { get; set; }
        public string VenueLongitude { get; set; }
        public string VenueLatitude { get; set; }

        public string Facility { get; set; }
        public string FacilityImage { get; set; }
        public string FacilityLocation { get; set; }
        public string FacilityLongitude { get; set; }
        public string FacilityLatitude { get; set; }

        public string Branch { get; set; }
        public string BranchImage { get; set; }
        public string BranchLocation { get; set; }
        public string BranchLongitude { get; set; }
        public string BranchLatitude { get; set; }
    }
}
