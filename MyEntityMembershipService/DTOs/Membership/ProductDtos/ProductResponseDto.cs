using Common.RabbitMq.Membership.Enums;

namespace MyEntityMembershipService.DTOs.Membership.ProductDtos
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductFilterResponseDto
    {
        public int? TenantId { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public ProductType ProductType { get; set; }

    }
}
