namespace MyEntityMembershipService.Data
{
    public class SharedEntity
    {
        public DateTime? CreationDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreatedByUsername { get; set; }
    }
}
