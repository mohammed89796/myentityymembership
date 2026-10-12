using MyEntityMembershipService.Data;


namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class UpdateFacilityDto : BaseEntity
    {
        public string name { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
