using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class PackageScopeConfiguration : IEntityTypeConfiguration<PackageScope>
    {
        public void Configure(EntityTypeBuilder<PackageScope> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
