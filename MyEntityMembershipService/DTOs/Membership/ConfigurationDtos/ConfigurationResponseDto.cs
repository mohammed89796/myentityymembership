


namespace MyEntityMembershipService.DTOs.Membership.ConfigurationDtos
{
    public interface IConfigurationResponseBase { }

    public class ConfigurationResponseDto : IConfigurationResponseBase
    {
        public string FieldHeader { get; set; }
        public string FieldName { get; set; }
        public int FieldtypeID { get; set; }
        public string FieldtypeName { get; set; }
        public string? FieldResponseString { get; set; }
        public bool? FieldResponseBool { get; set; }
    }

    public class MemberConfigurationResponseDto : IConfigurationResponseBase
    {
        public string FieldName { get; set; }
        public int FieldtypeID { get; set; }
        public string FieldtypeName { get; set; }
        public int MemberID { get; set; }
        public int? FieldResponse { get; set; }
    }
}
