namespace MyEntityMembershipService.DTOs.Entity.TenantReviewDtos;

public class TenantReviewSummaryDto
{
    public int TenantId { get; set; }
    public int TotalReviews { get; set; }      
    public double AverageRating { get; set; }  
    public List<RatingBucketDto> Breakdown { get; set; } = new();
}