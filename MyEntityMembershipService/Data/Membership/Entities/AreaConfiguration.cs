namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class AreaConfiguration : BaseEntity
    {
        public double FormFee { get; set; }

        public Form Form { get; set; }
        public int FormId { get; set; }
        public double AreaFee { get; set; }
        public double AreaVAT { get; set; }
        public double AreaOperatingFees { get; set; }

        public RequestType Operation { get; set; }
        public int OperationId { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; }

        public ICollection<PaymentBatch> PaymentBatches { get; set; } = [];
    }
}
