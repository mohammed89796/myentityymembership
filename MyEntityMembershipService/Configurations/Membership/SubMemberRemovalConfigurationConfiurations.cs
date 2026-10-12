using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class SubMemberRemovalConfigurationConfigurations : IEntityTypeConfiguration<SubMemberRemovalConfiguration>
    {
        public void Configure(EntityTypeBuilder<SubMemberRemovalConfiguration> builder)
        {
            builder.HasKey(config => new {config.RemovalConfigurationId, config.SubMemberId});

            builder.HasOne(config => config.RemovalConfiguration)
                .WithMany(type => type.SubMemberRemovalConfigurations)
                .HasForeignKey(config => config.RemovalConfigurationId);

            builder.HasOne(config => config.MemberType)
                .WithMany(config => config.SubMemberRemovalConfigurations)
                .HasForeignKey(config => config.SubMemberId);
        }
    }
}
