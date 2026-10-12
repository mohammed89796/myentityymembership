namespace MyEntityMembershipService.DTOs.Entity.SeriesDtos
{
    public class EventToReturnDto
    {
        public int Id { get; set; }

        public string EventName { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string? Duration { get; set; }

        public string? Location { get; set; }

        public int? Price { get; set; }

        public int SeriesId { get; set; }

        public string SeriesName { get; set; }

        public List<EventDetailToReturnDto> EventDetails { get; set; } = new List<EventDetailToReturnDto>();
    }
}
