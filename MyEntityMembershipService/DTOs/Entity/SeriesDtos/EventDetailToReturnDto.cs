namespace MyEntityMembershipService.DTOs.Entity.SeriesDtos
{
	public class EventDetailToReturnDto
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Duration { get; set; }

		public string? Description { get; set; }

		public string? EventDetailPictureUrl { get; set; }

		public string EventName { get; set; }

		public int EventId { get; set; }
	}
}
