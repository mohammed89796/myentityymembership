namespace MyEntityMembershipService.DTOs.Entity.PostDtos
{
    public interface IPostDto
    {
        public string PostTitle { get; set; }

        public string Description { get; set; }

        public List<IFormFile>? PostPictures { get; set; }

        public DateOnly? PublishDate { get; set; }

        public int TenantId { get; set; }

        public string CreatedBy { get; set; }

    }
}
