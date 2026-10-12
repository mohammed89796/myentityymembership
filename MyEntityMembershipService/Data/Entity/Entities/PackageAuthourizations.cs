namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class PackageAuthourizations:SharedEntity
    {
        public int PackageId { get; set; }
        public virtual Package Package { get; set; }

        public int RequestAuthourizationId { get; set; }
    }
}
