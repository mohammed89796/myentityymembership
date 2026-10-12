using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class ConfigurationRequestConfigurations : IEntityTypeConfiguration<Configuration>
    {
        public void Configure(EntityTypeBuilder<Configuration> builder)
        {


            builder.HasOne(config => config.RequestType)
                       .WithOne(type => type.Configuration)
                       .HasForeignKey<Configuration>(config => config.RequestTypeId);

            builder.HasMany(config => config.PackageConfigurationSections)
               .WithOne(type => type.Configuration)
               .HasForeignKey(field => field.ConfigurationId);



        }
    }

}