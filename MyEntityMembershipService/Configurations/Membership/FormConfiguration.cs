using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class FormConfiguration : IEntityTypeConfiguration<Form>
    {
        public void Configure(EntityTypeBuilder<Form> builder)
        {
            builder.HasOne(form => form.RequestType)
                .WithMany(type => type.Forms)
                .HasForeignKey(form => form.RequestTypeId);


            builder.HasMany(config => config.NewMembershipConfigurations)
                .WithOne(form => form.Form)
                .HasForeignKey(config => config.FormId);


            builder.HasOne(form => form.ParentForm)
            .WithMany(form => form.ChildForms)
            .HasForeignKey(form => form.ParentFormId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
