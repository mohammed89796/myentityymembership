using System.ComponentModel.DataAnnotations.Schema;
using MyEntityMembershipService.Data;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class BranchOperatingHours : BaseEntity
    {
        public TimeOnly? SundayOpenAt { get; set; }
        public TimeOnly? SundayCloseAt { get; set; }
        public bool? SundayIsWorking { get; set; }
        public TimeOnly? MondayOpenAt { get; set; }
        public TimeOnly? MondayCloseAt { get; set; }
        public bool? MondayIsWorking { get; set; }
        public TimeOnly? TuesdayOpenAt { get; set; }
        public TimeOnly? TuesdayCloseAt { get; set; }
        public bool? TuesdayIsWorking { get; set; }
        public TimeOnly? WednesdayOpenAt { get; set; }
        public TimeOnly? WednesdayCloseAt { get; set; }
        public bool? WednesdayIsWorking { get; set; }
        public TimeOnly? ThursdayOpenAt { get; set; }
        public TimeOnly? ThursdayCloseAt { get; set; }
        public bool? ThursdayIsWorking { get; set; }
        public TimeOnly? FridayOpenAt { get; set; }
        public TimeOnly? FridayCloseAt { get; set; }
        public bool? FridayIsWorking { get; set; }
        public TimeOnly? SaturdayOpenAt { get; set; }
        public TimeOnly? SaturdayCloseAt { get; set; }
        public bool? SaturdayIsWorking { get; set; }
        public int BranchID { get; set; }

        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }

    }
}
