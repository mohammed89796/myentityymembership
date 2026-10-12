namespace MyEntityMembershipService.DTOs.Membership
{
    public class FileDto
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }

}
