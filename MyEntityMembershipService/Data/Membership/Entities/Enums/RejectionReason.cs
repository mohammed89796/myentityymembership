using System.ComponentModel.DataAnnotations;

namespace MyEntityMembershipService.Data.Membership.Entities.Enums
{
    public enum RejectionReason
    {
        [Display(Name = "البيانات غير كاملة")]
        DataNotComplete = 0,

        [Display(Name = "البيانات لا تستوفي الشروط")]
        InvalidDocumentation = 1,

        [Display(Name = "البيانات غير مطابقة للشخص")]
        MismatchedDocumentation = 2
    }
}
