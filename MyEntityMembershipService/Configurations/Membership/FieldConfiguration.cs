using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class FieldConfiguration : IEntityTypeConfiguration<Field>
    {
        public void Configure(EntityTypeBuilder<Field> builder)
        {

            builder.HasOne(field => field.FieldType)
                .WithMany(type => type.Fields)
                .HasForeignKey(field => field.FieldTypeId);

            builder.HasMany(field => field.DropdownValues)
                .WithOne(value => value.Field)
                .HasForeignKey(value => value.FieldId);

            builder.HasOne(field => field.UnitType)
                   .WithMany(type => type.Fields)
                   .HasForeignKey(field => field.UnitTypeId);
        }
    }
}
