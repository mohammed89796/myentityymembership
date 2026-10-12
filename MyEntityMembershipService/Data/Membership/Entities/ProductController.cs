using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.DTOs.Membership.ProductDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Services.Membership.ProductService;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    [Route("api/{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class ProductController:ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        public async Task<IResult> GetAllProducts([FromQuery] ProductFilterResponseDto dto)
        {
            dto.TenantId = User.GetParsedTenantId();
            var result = await _productService.GetAllProducts(dto);
             return result.GetResponse();
        }
    }
}
