using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class PaakageMemberFormConfig : IEntityTypeConfiguration<PaakageMemberForm>
    {
        public void Configure(EntityTypeBuilder<PaakageMemberForm> builder)
        {
            builder.HasKey(b => new { b.PackageId, b.FormId, b.MemberTypeId });


            builder.HasOne(config => config.Form)
                       .WithMany(type => type.PaakageMemberForms)
                       .HasForeignKey(config => config.FormId);

            builder.HasOne(config => config.MemberType)
                     .WithMany(type => type.PaakageMemberForms)
                     .HasForeignKey(config => config.MemberTypeId);





        }
    }

}