using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasOne(e => e.Series)
                               .WithMany(s => s.Events)
                               .HasForeignKey(e => e.SeriesId)
                               .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
