using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Entity
{
    public class SubMemberCategoryConfiguration : IEntityTypeConfiguration<SubMemberCategory>
    {
        public void Configure(EntityTypeBuilder<SubMemberCategory> builder)
        {
            builder.HasOne(x => x.MemberType)
               .WithMany(y=>y.SubMemberCategories)
               .HasForeignKey(x => x.MemberTypeId)
               .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
