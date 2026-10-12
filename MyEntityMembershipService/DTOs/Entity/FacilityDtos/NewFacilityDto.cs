


using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEntityMembershipService.DTOs.Entity.FacilityDtos
{
    public class NewFacilityDto
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string name { get; set; }
        public string Image { get; set; }
        public int FacilityTypeId { get; set; }
        public string? FacilityType { get; set; }
        public string? Description { get; set; }
        public string CountryCodeID { get; set; }
        public int PhoneNumber { get; set; }
        public int FacilityPremisesTypeID { get; set; }
        //public string  FacilityLocation { get; set; }
        public List<NewVenueDto>? Venues { get; set; } = new List<NewVenueDto>();



    }
    public class NewFacilityDashboardDto
    {
        public int Id { get; set; }
        public string name { get; set; }
    }

    public class NewFacilityDashboardSearchDto
    {
        public string? SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }



}
