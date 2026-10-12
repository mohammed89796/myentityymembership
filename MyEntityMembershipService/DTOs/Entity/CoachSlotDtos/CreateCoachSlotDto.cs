using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEntityMembershipService.DTOs.Entity.CoachSlotDtos
{
    public class CreateCoachSlotDto
    {
        public int Capacity { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayOfWeek Day { get; set; }
    }
}