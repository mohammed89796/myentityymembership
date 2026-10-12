using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class FieldResponseConfiguration : IEntityTypeConfiguration<FieldResponse>
    {
        public void Configure(EntityTypeBuilder<FieldResponse> builder)
        {
            builder.HasOne(fieldResponse => fieldResponse.FormUserResponse)
                   .WithMany(userResponse => userResponse.FieldsResponse)
                   .HasForeignKey(fieldResponse => fieldResponse.FormUserResponseId);

            builder.HasOne(fieldResponse => fieldResponse.Field)
                   .WithMany(field => field.FieldResponses)
                   .HasForeignKey(fieldResponse => fieldResponse.FieldId);

            builder.HasOne(response => response.Unit)
                   .WithMany(unit => unit.FieldResponses)
                   .HasForeignKey(response => response.UnitId);
        }
    }
}
