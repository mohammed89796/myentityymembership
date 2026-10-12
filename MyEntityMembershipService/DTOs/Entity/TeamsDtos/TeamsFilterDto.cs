using System.Security.Cryptography.X509Certificates;

namespace MyEntityMembershipService.DTOs.Entity.TeamsDtos
{
    public class TeamsFilterDto
    {
        public List<int>? SportList { get; set; }
        public string? Gender { get; set; } 
        public string? SortField { get; set; }
        public string? SearchText { get; set; }

    }

}
