using MyEntityMembershipService.Data.Membership.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.DTOs.Membership.Swimming
{
    public class UpdateMembershipDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public Gender? Gender { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? NationalId { get; set; }

        public DateTime? BirthDate { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public IFormFile? UserPhoto { get; set; }
        public InjuryStatus? InjuryStatus { get; set; }
    }
}
