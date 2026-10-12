namespace MyEntityMembershipService.Data.Membership.Entities.Enums
{
    public enum RequestStatus
    {
        RequestResolved = 1,
        Rejected = 2,
        Pending = 3,
        PaymentComplete = 4,
        PendingPayment = 5,
        Draft = 6,
        PendingReview = 7
    }
}