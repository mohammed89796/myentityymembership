using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEntityMembershipService.Data.Entity.Entities;


namespace MyEntityMembershipService.Configurations.Entity
{
    public class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
    {
        public void Configure(EntityTypeBuilder<Choice> builder)
        {
            builder
            .HasOne(c => c.Poll)
            .WithMany(p => p.Choices)
            .HasForeignKey(c => c.PollId)
            .OnDelete(DeleteBehavior.Cascade);



            builder.HasMany(c => c.UserChoices)
          .WithOne(uc => uc.Choice)
          .HasForeignKey(uc => uc.ChoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
