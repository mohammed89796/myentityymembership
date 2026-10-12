using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEntityMembershipService.DTOs.Entity.CoachSlotDtos
{
    public class UpdateCoachSlotDto
    {
        public int CoachSlotId { get; set; }
        public int TenantAdminId { get; set; }
        public int Capacity { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayOfWeek Day { get; set; }
    }
}