using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class MemberRequestTypeConfiguration : IEntityTypeConfiguration<MemberRequestType>
    {
        public void Configure(EntityTypeBuilder<MemberRequestType> builder)
        {
            builder.HasOne(mrt => mrt.RequestType)
                .WithMany(rt => rt.MemberRequestTypes)
                .HasForeignKey(mrt => mrt.RequestTypeId)
                .IsRequired();



            builder.HasOne(mrt => mrt.MemberType)
                .WithMany(mt => mt.MemberRequestTypes)
                .HasForeignKey(mrt => mrt.MemberTypeId)
                .IsRequired();
        }
    }
}
