using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.ProductDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Linq.Expressions;

namespace MyEntityMembershipService.Services.Membership.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Response<PaginationToReturnDto<ProductResponseDto>>> GetAllProducts(ProductFilterResponseDto dto)
        {
            try
            {
                if (!dto.TenantId.HasValue)
                {
                    Log.Warning("TenantId is missing in the request");
                    return new ResponseHandler().BadRequest<PaginationToReturnDto<ProductResponseDto>>("TenantId is required.");

                }
                int productTypeValue = (int)dto.ProductType;

                Expression<Func<Product, ProductResponseDto>> selector = p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name
                };

                Expression<Func<Product, bool>> criteria = p => (int)p.ProductType == productTypeValue && p.GlobalTenantId == dto.TenantId;

                var productSpec = new GeneralSpecifications<Product>(criteria, dto.PageNumber, dto.PageSize);
                var products = await _unitOfWork.Repository<Product>().GetAllWithSpecificationsAndProjection(productSpec, selector);

                Log.Information("Fetched {Count} products of type {ProductType}", products.Count(), dto.ProductType);

                var totalCount = await _unitOfWork.Repository<Product>().GetCountByCriteriaAsync(criteria);

                var totalPages = (int)Math.Ceiling((double)totalCount / dto.PageSize);

                var paginationResult = new PaginationToReturnDto<ProductResponseDto>(
                    count: totalCount,
                    totalPages: totalPages,
                    pageSize: dto.PageSize,
                    pageNumber: dto.PageNumber,
                    data: products.ToList()
                );

                return new ResponseHandler().Success(paginationResult);
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while fetching products: {Message}", ex.Message);
                return new ResponseHandler().BadRequest<PaginationToReturnDto<ProductResponseDto>>("An error occurred while fetching products.");
            }
        }
    }
}
