using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Data.Membership.Entities
{
    public class CoachSlots : BaseEntity
    {
        public int Capacity { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayOfWeek Day { get; set; }
        public int TenantAdminId { get; set; }
        public TenantAdmin TenantAdmin { get; set; }
        
    }
}