using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class City : BaseEntity
    {
        public string Name { get; set; }
        public int StateID { get; set; }

        [ForeignKey("StateID")]
        public virtual State State { get; set; }
    }
}
