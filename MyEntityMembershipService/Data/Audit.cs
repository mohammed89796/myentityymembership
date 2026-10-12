namespace MyEntityMembershipService.Data
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string OriginalValuesJson { get; set; }
        public string CurrentValuesJson { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedOn { get; set; }

        public int EntityId { get; set; }
    }
}
