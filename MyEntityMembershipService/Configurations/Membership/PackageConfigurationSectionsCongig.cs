using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class PackageConfigurationSectionsCongig : IEntityTypeConfiguration<PackageConfigurationSection>
    {
        public void Configure(EntityTypeBuilder<PackageConfigurationSection> builder)
        {

            builder.HasMany(field => field.ConfigurationFields)
                .WithOne(type => type.PackageConfigurationSection)
                .HasForeignKey(field => field.PackageConfigurationSectionId);


        }
    }

}
