using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class FormMemberTypeConfiguration : IEntityTypeConfiguration<FormMemberType>
    {
        public void Configure(EntityTypeBuilder<FormMemberType> builder)
        {
            builder.HasOne(item => item.MemberType)
                .WithMany(memberType => memberType.Forms)
                .HasForeignKey(item => item.MemberTypeId);

            builder.HasOne(item => item.Form)
                .WithMany(form => form.MemberTypes)
                .HasForeignKey(item => item.FormId);

            builder.HasKey(item => new { item.FormId, item.MemberTypeId });
        }
    }
}
