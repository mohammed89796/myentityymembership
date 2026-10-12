namespace MyEntityMembershipService.DTOs.Membership.AreaDtos
{
    public class GetAreasDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? NumberOfEntities { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
