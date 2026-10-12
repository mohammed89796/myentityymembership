namespace MyEntityMembershipService.DTOs.Entity.SeriesDtos
{
    public class EventDetailDto
    {
        public string Title { get; set; }

        public string Duration { get; set; }

        public string? Description { get; set; }

        public IFormFile? EventDetailPicture { get; set; }

        public int EventId { get; set; }
    }
}
