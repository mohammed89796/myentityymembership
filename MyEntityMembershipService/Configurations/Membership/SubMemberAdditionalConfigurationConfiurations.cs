using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class SubMemberAdditionalConfigurationConfiurations : IEntityTypeConfiguration<SubMemberAdditionalConfiguration>
    {
        public void Configure(EntityTypeBuilder<SubMemberAdditionalConfiguration> builder)
        {
            builder.HasKey(b => new { b.AdditionalConfigurationId, b.SubMemberId });

            builder.HasOne(config => config.AdditionalConfiguration)
                     .WithMany(type => type.SubMemberAdditionalConfigurations)
                     .HasForeignKey(config => config.AdditionalConfigurationId);

            builder.HasOne(config => config.MemberType)
                     .WithMany(type => type.SubMemberAdditionalConfigurations)
                     .HasForeignKey(config => config.SubMemberId);

        }
    }

}