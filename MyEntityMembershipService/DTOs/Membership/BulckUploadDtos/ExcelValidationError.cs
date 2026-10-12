namespace MyEntityMembershipService.DTOs.Membership.BulckUploadDtos
{
    public class ExcelValidationError
    {
        public string Cell { get; set; }         // e.g., "B2"
        public string Message { get; set; }      // e.g., "Invalid value for FedrationEntityTypeId"
    }

}
