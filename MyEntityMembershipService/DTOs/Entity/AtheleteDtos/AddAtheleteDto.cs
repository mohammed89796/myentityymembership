using MyEntityMembershipService.Data.Membership.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.DTOs.Entity.AtheleteDtos
{
    public class AddAtheleteDto : ICommonData
    {

        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string FileNumber { get; set; }
        public int TenantId { get; set; }
        public Gender Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public DateOnly DateOfAcquisition { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid egyptian mobile number.")]
        public string MobileNumber { get; set; }

        public string? SchoolWorkEntity { get; set; }

        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Passport number must contain only letters and digits.")]
        public string? PassportNumber { get; set; }

        public DateOnly DateOfExpiry { get; set; }
        public DateOnly? DateOfIssuance { get; set; }

        [RegularExpression(@"^(2|3)\d{13}$", ErrorMessage = "National ID must be 14 digits")]
        public string? NationalId { get; set; }

        [RegularExpression(@"^0\d{2,3}\d{7}$", ErrorMessage = "Invalid Egyptian home number.")]
        public string HomeNumber { get; set; }
        public int? BeltId { get; set; }
        public IFormFile? PersonalPhoto { get; set; }
        public IFormFile? PersonalPhotoWithStamp { get; set; }
        public IFormFile? NationalIdPhoto { get; set; }
        public IFormFile? BeltCertificate { get; set; }
    }
}
