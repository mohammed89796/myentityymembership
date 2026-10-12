using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class RequestStatusHistoriesConfigurations : IEntityTypeConfiguration<RequestStatusHistory>
    {
        public void Configure(EntityTypeBuilder<RequestStatusHistory> builder)
        {
            builder.HasKey(b => new { b.MembershipRequestId, b.RequestStatusId });

            builder.HasOne(request => request.MembershipRequest)
                   .WithMany(type => type.RequestStatusHistories)
                   .HasForeignKey(request => request.MembershipRequestId);


        }
    }
}

