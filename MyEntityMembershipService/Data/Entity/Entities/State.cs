using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.Data.Entity.Entities
{
    public class State : BaseEntity
    {
        public string Name { get; set; }
        public int CountryID { get; set; }

        [ForeignKey("CountryID")]
        public virtual Country Country { get; set; }
        public ICollection<City> City { get; set; }

       
    }
}
