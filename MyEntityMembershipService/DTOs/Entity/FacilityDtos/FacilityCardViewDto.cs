namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class FacilityListDetailsDto
    {
        public int TotalNumber { get; set; }
        public int NumberOfPages { get; set; }
        public List<FacilityCardViewDto> Facilities { get; set; }
    }
    public class FacilityCardViewDto
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string FacilityName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
