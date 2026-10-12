using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class RequestTypeCategoryConfig : IEntityTypeConfiguration<RequestTypeCategory>
    {
        public void Configure(EntityTypeBuilder<RequestTypeCategory> builder)
        {
            builder.HasMany(tc => tc.RequestTypes)
                   .WithOne(rt => rt.RequestTypeCategory)
                   .HasForeignKey(rt => rt.RequestTypeCategoryId);
        }
    }
}
