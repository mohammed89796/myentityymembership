using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.SeriesDtos
{
    public class SeriesDto
    {

        public string SeriesName { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public int? Price { get; set; }

        public IFormFile? SeriesPicture { get; set; }

        public RegistrationProcess? RegistrationProcess { get; set; }

        public RegistrationType? RegistrationType { get; set; }

        public string? AssignedAdmin { get; set; }

        public string? Description { get; set; }


    }
}
