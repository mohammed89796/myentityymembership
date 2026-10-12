namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class RenwalOption:BaseEntity
    {
        public int RenewalConfigurationId { get; set; }

        public virtual RenewalConfiguration RenewalConfiguration { get; set; }
        public int Duration { get; set; }
        public int DurationUnitId { get; set; }

        public virtual Unit DurationUnit { get; set; }
        public double Price { get; set; }
        public int PriceUnitId { get; set; }
        public virtual Unit PriceUnit { get; set; }






    }
}
