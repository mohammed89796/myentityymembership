

using MyEntityMembershipService.Data.Membership.Entities.Enums;

namespace MyEntityMembershipService.DTOs.Membership.FreezeConfigurtionDtos
{
    public class FreezeConfigurationDto
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public int FreezeLimit { get; set; }
        public bool CanExceed { get; set; }
        public double PricePerAdditionalDay { get; set; }

        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public PaymentProceduresType PaymentProceduresType { get; set; }
    }
}
