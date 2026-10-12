using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class FormFieldsConfiguration : IEntityTypeConfiguration<FormFields>
    {
        public void Configure(EntityTypeBuilder<FormFields> builder)
        {
            builder.HasOne(item => item.Form)
                .WithMany(form => form.FormFields)
                .HasForeignKey(item => item.FormId);

            builder.HasOne(item => item.Field)
                .WithMany(field => field.FormFields)
                .HasForeignKey(item => item.FieldId);

            builder.HasKey(item => new { item.FieldId, item.FormId });
        }
    }
}
