using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.ProductDtos;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.ProductService
{
    public interface IProductService
    {
        Task<Response<PaginationToReturnDto<ProductResponseDto>>> GetAllProducts(ProductFilterResponseDto dto);
    }
}
