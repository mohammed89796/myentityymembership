using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Data.Membership.Entities.Membership>
    {
        public void Configure(EntityTypeBuilder<Data.Membership.Entities.Membership> builder)
        {
           
             builder.HasOne(m => m.ParentMembership)
            .WithMany(m => m.SubMembers)
            .HasForeignKey(m => m.ParentMembershipId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents cascading delete to avoid deleting all sub-members

            builder.HasOne(m => m.MemberType)
                    .WithMany(type => type.Memberships)
                    .HasForeignKey(m => m.MemberTypeId);
            builder.HasOne(m => m.Package)
                .WithMany(type => type.Memberships)
                .HasForeignKey(m => m.PackageId);

            builder.HasMany(r => r.InjuryReports)
               .WithOne(t => t.Membership)
               .HasForeignKey(r => r.MembershipId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.MembershipAccesses)
            .WithOne(t => t.Membership)
            .HasForeignKey(r => r.MembershipId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.MemberSportLevels)
            .WithOne(t => t.Membership)
            .HasForeignKey(r => r.MembershipId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}