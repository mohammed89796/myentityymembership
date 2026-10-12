using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class ConfigurationFieldConfigurations : IEntityTypeConfiguration<ConfigurationField>
    {
        public void Configure(EntityTypeBuilder<ConfigurationField> builder)
        {

            builder.HasOne(field => field.FieldType)
                .WithMany(type => type.ConfigurationFields)
                .HasForeignKey(field => field.FieldTypeId);

            builder.HasMany(field => field.DropdownValues)
              .WithOne(type => type.ConfigurationField)
              .HasForeignKey(field => field.ConfigurationFieldId);


        }
    }
    
}
