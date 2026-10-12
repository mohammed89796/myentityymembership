namespace MyEntityMembershipService.DTOs.Entity.AtheleteDtos
{
    public interface ICommonData
    {

        public string? Email { get; set; }


        public string? MobileNumber { get; set; }


        public string? PassportNumber { get; set; }


        public string? HomeNumber { get; set; }


        public string? NationalId { get; set; }

        public IFormFile? PersonalPhoto { get; set; }
        public IFormFile? PersonalPhotoWithStamp { get; set; }
        public IFormFile? NationalIdPhoto { get; set; }
        public IFormFile? BeltCertificate { get; set; }
    }
}
