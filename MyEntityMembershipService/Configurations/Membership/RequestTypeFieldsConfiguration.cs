using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Configurations.Membership
{
    public class RequestTypeFieldsConfiguration : IEntityTypeConfiguration<RequestTypeFields>
    {
        public void Configure(EntityTypeBuilder<RequestTypeFields> builder)
        {
            builder.HasOne(item => item.Field)
                    .WithMany(field => field.RequestTypeFields)
                    .HasForeignKey(item => item.FieldId);

            builder.HasOne(item => item.RequestType)
                    .WithMany(requestType => requestType.Fields)
                    .HasForeignKey(item => item.RequestTypeId);

            builder.HasKey(item => new { item.FieldId, item.RequestTypeId });

            builder.HasOne(item => item.FieldCategory)
                   .WithMany(category => category.RequestTypeFields)
                   .HasForeignKey(item => item.FieldCategoryId);
        }
    }
}
