using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEntityMembershipService.DTOs.Entity.CoachSlotDtos
{
    public class CoachSlotDto
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayOfWeek Day { get; set; }
    }
}