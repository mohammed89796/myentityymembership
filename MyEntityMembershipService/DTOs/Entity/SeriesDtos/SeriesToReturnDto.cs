using MyEntityMembershipService.Data.Entity.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Entity.SeriesDtos
{
    public class SeriesToReturnDto
    {
        public int Id { get; set; }

        public string SeriesName { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? DayNameOfStartDate { get; set; }

        public string? MonthNameOfStartDate { get; set; }

        public string? DayNameOfEndDate { get; set; }
        public string? MonthNameOfEndDate { get; set; }

        public int? Price { get; set; }

        public string? SeriesPictureUrl { get; set; }

        public RegistrationProcess? RegistrationProcess { get; set; }

        public string? RegistrationProcessName { get; set; }

        public RegistrationType? RegistrationType { get; set; }

        public string? RegistrationTypeName { get; set; }

        public string? AssignedAdmin { get; set; }

        public string? Description { get; set; }

        public List<EventToReturnDto> Events { get; set; } = new List<EventToReturnDto>();
    }
}
