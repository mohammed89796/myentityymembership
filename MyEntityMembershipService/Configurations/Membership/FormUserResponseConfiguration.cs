using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class FormUserResponseConfiguration : IEntityTypeConfiguration<FormUserResponse>
    {
        public void Configure(EntityTypeBuilder<FormUserResponse> builder)
        {
            builder.HasOne(response => response.Form)
                   .WithMany(form => form.FormResponses)
                   .HasForeignKey(response => response.FormId);

            builder.HasOne(response => response.Membership)
                   .WithMany(membership => membership.FormUserResponses)
                   .HasForeignKey(response => response.MembershipId);

            builder.HasOne(response => response.MembershipRequest)
                   .WithOne(request => request.FormUserResponse)
                   .HasForeignKey<FormUserResponse>(response => response.MembershipRequestId);

            builder.HasOne(response => response.SubMemberMembership)
                   .WithMany(membership => membership.SubMemberFormUserResponses)
                   .HasForeignKey(response => response.SubMemberId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(response => response.MemberType)
                    .WithMany(type => type.FormUserResponses)
                    .HasForeignKey(response => response.MemberTypeId);

        }
    }
}
