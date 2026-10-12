using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class FreezeConfigurationConfigurations : IEntityTypeConfiguration<FreezeConfiguration>
    {
        public void Configure(EntityTypeBuilder<FreezeConfiguration> builder)
        {
            
            builder.HasIndex(b => b.PackageId).IsUnique();
        }
    }

    public class AdditionalConfigurationConfigurations : IEntityTypeConfiguration<AdditionalConfiguration>
    {
        public void Configure(EntityTypeBuilder<AdditionalConfiguration> builder)
        {
            

            builder.HasIndex(b => b.PackageId).IsUnique();
        }
    }

    public class RenewalConfigurationConfigurations : IEntityTypeConfiguration<RenewalConfiguration>
    {
        public void Configure(EntityTypeBuilder<RenewalConfiguration> builder)
        {
           

            builder.HasIndex(b => b.PackageId).IsUnique();

            builder.HasMany(op => op.RenewalOptions)
                   .WithOne(type => type.RenewalConfiguration)
                   .HasForeignKey(request => request.RenewalConfigurationId);
        }
    }

    public class RemovalConfigurationConfigurations : IEntityTypeConfiguration<RemovalConfiguration>
    {
        public void Configure(EntityTypeBuilder<RemovalConfiguration> builder)
        {
            

            builder.HasIndex(b => b.PackageId).IsUnique();
        }
    }

    public class CancellationConfigurationConfigurations : IEntityTypeConfiguration<CancellationConfiguration>
    {
        public void Configure(EntityTypeBuilder<CancellationConfiguration> builder)
        {
            

            builder.HasIndex(b => b.PackageId).IsUnique();
        }
    }
}