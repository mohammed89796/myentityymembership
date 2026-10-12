namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class FacilityWrapperDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public string FacilityLocation { get; set; }
        public string FacilityLatitude { get; set; }
        public string FacilityLongitude { get; set; }
    }
}
