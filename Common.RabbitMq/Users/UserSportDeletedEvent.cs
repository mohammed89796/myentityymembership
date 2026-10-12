using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RabbitMq.Users
{
    public class UserSportDeletedEvent
    {
        public string UserId { get; set; }
        public int SportId { get; set; }
        public int TenantId { get; set; }
    }
}
