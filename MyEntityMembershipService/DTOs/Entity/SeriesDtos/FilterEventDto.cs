namespace MyEntityMembershipService.DTOs.Entity.SeriesDtos
{
    public class FilterEventDto
    {
        public string? EventName { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string? Duration { get; set; }

        public string? Location { get; set; }

        public int? Price { get; set; }

    }
}
