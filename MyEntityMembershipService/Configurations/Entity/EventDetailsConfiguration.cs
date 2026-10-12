using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class EventDetailsConfiguration : IEntityTypeConfiguration<EventDetail>
    {
        public void Configure(EntityTypeBuilder<EventDetail> builder)
        {
            builder
                .HasOne(ed => ed.Event)
                .WithMany(e => e.EventDetails)
                .HasForeignKey(ed => ed.EventId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
