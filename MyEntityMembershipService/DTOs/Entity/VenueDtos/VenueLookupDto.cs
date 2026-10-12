namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class VenueLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }



    public class VenueLookupSearchDto
    {
        public string? SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
