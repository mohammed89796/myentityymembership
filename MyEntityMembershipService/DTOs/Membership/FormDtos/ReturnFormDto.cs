namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class ReturnFormDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public double Fees { get; set; }
        // Updated Form Fees As in Configuration
        public double UpdatedFees { get; set; }
        public double OperationalFees { get; set; }
        public string? WorkflowSelection { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestTypeName { get; set; }
        public string? CreatedBy { get; set; }
        public string? UserName { get; set; } // Created By User Name
        public DateTime? CreationDate { get; set; }
        public List<FieldDto> Fields { get; set; }
        public bool IsDraft { get; set; }
    }


    public class FormResponseSearchDto
    {
        public int? TenantId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }



    public class FormResponseDto
    {
        public int FormId { get; set; }
        public string? Title { get; set; }
        public bool IsDraft { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? CreatedBy { get; set; }

    }
}
