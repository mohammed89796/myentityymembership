

using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class GetRescheduleMemberDetails
    {
        public int BookingID { get; set; }
        public string BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string MemberName { get; set; }
        public string MemberID { get; set; }
        public string PhoneNumber { get; set; }
        public string VenueName { get; set; }
        public string VenueImage { get; set; }
        public string BranchName { get; set; }
        public double PricePaided { get; set; }

    }
    public class GetRescheduleOutOfServiceDetails
    {
        public int BookingID { get; set; }
        public string BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string VenueName { get; set; }
        public string VenueImage { get; set; }
        public string BranchName { get; set; }
    }
    public class GetRescheduleEventDetails
    {
        public int BookingID { get; set; }
        public string BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public bool? IsNoneMember { get; set; }
        public string VenueName { get; set; }
        public string VenueImage { get; set; }
        public string BranchName { get; set; }
    }
}
