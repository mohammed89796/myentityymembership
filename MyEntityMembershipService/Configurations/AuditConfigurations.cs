//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using MyEntityMembershipService.Data;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace MyEntityMembershipService.Configurations
//{
//    public class AuditConfigurations : IEntityTypeConfiguration<MyEntityMembershipService.Data.AuditLog>

//    {
//        public void Configure(EntityTypeBuilder<AuditLog> builder)
//        {



//            var options = new JsonSerializerOptions
//            {

//                PropertyNameCaseInsensitive = true,
//                ReferenceHandler = ReferenceHandler.IgnoreCycles,
//                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

//            };

//            builder.Property(x => x.OriginalValuesJson).HasConversion(
//                v => JsonSerializer.Serialize(v, options),
//                v => v
//            ).HasColumnType("json");

//            builder.Property(x => x.CurrentValuesJson).HasConversion(
//                v => JsonSerializer.Serialize(v, options),
//                v => v
//            ).HasColumnType("json");
//        }
//    }
//}
