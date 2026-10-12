

using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.DTOs.Entity.VenueDtos
{
    public class VenueUsageRequestModel
    {
        public int? BranchID { get; set; }
        public int? FacilityID { get; set; }
        public int? Month { get; set; } 
        public int? Year { get; set; } 

    }
    public class VenueUsageResponseDTO
    {
        public double MaxUsage { get; set; }
        public double MinUsage { get; set; }
        public int Interval { get; set; }
        public List<MonthlyUsageDTO> MonthlyUsage { get; set; }

    }
    public class MonthlyUsageDTO
    {   
        public string MonthName { get; set; }
        public List<VenueUsageDTO>? VenueUsage { get; set; }
    }
    public class VenueUsageDTO
    {   
        public int VenueID { get; set; }
        public string VenueName { get; set; }
        public int Usage { get; set; }

    }

   
}
