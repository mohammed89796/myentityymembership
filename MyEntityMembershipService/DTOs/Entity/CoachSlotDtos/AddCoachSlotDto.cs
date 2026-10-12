using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEntityMembershipService.DTOs.Entity.CoachSlotDtos
{
    public class AddCoachSlotDto
    {
        public int TenantAdminId { get; set; }
        public List<CreateCoachSlotDto> CoachSlots { get; set; } = [];
    }
}
