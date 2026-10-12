namespace MyEntityMembershipService.DTOs.Membership.EntityDtos
{
    public class PagedResult<T>
    {
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }
    }
}