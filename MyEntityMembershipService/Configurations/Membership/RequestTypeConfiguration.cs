using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class RequestTypeConfiguration : IEntityTypeConfiguration<PackageRequestAuthourizations>
    {
        public void Configure(EntityTypeBuilder<PackageRequestAuthourizations> builder)
        {
            builder.HasKey(item => new { item.RequestAuthourizationId, item.PackageId });

            builder.HasOne(requestType => requestType.RequestType)
                .WithMany(type => type.PackageRequestAuthourizations)
                .HasForeignKey(field => field.RequestAuthourizationId);
            builder.HasOne(requestType => requestType.Package)
              .WithMany(type => type.PackageRequestAuthourizations)
              .HasForeignKey(field => field.PackageId);
        }
    }
}
