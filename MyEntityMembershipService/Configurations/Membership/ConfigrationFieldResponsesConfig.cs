using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class ConfigrationFieldResponsesConfig : IEntityTypeConfiguration<ConfigrationFieldResponse>
    {
        public void Configure(EntityTypeBuilder<ConfigrationFieldResponse> builder)
        {
            builder.HasKey(b => new { b.ConfigurationId, b.FieldId,b.PackageId });


            builder.HasOne(config => config.Configuration)
                       .WithMany(type => type.ConfigrationFieldResponses)
                       .HasForeignKey(config => config.ConfigurationId);

            builder.HasOne(config => config.ConfigurationField)
                     .WithMany(type => type.ConfigrationFieldResponses)
                     .HasForeignKey(config => config.FieldId);





        }
    }

}