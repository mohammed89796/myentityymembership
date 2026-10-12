namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class PackageRequestAuthourizations:SharedEntity
    {
        public int PackageId { get; set; }
        public virtual Package Package { get; set; }
        public int RequestAuthourizationId { get; set; }
        public RequestType RequestType { get; set; }



    }
}
