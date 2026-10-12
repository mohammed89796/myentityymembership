namespace MyEntityMembershipService.DTOs
{
    public class AuditLogDto<T>
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public T OriginalValuesJson { get; set; }
        public T CurrentValuesJson { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public int EntityId { get; set; }
    }
}
