namespace MyEntityMembershipService.DTOs.Entity.BranchDtos
{
    public class EditBranchDto
    {
        public int BranchID { get; set; }

        public string Name { get; set; }
        public IFormFile? BranchImg { get; set; }
        public List<IFormFile>? BranchUploads { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        public string Location { get; set; }
        public int PostalCode { get; set; }
        public string BranchManagerID { get; set; }
        public string Description { get; set; }
        public BranchLocationDto BranchLocation { get; set; }
        public EditBranchOperatingHoursDto BranchOperatingHours { get; set; }

        // Contact information
        public string CountryCodeID { get; set; }
        public int PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public List<int> BranchServices { get; set; }
    }

    public class EditBranchOperatingHoursDto
    {
        public string? SundayOpenAt { get; set; }
        public string? SundayCloseAt { get; set; }
        public bool? SundayIsWorking { get; set; }

        public string? MondayOpenAt { get; set; }
        public string? MondayCloseAt { get; set; }
        public bool? MondayIsWorking { get; set; }

        public string? TuesdayOpenAt { get; set; }
        public string? TuesdayCloseAt { get; set; }
        public bool? TuesdayIsWorking { get; set; }

        public string? WednesdayOpenAt { get; set; }
        public string? WednesdayCloseAt { get; set; }
        public bool? WednesdayIsWorking { get; set; }

        public string? ThursdayOpenAt { get; set; }
        public string? ThursdayCloseAt { get; set; }
        public bool? ThursdayIsWorking { get; set; }

        public string? FridayOpenAt { get; set; }
        public string? FridayCloseAt { get; set; }
        public bool? FridayIsWorking { get; set; }

        public string? SaturdayOpenAt { get; set; }
        public string? SaturdayCloseAt { get; set; }
        public bool? SaturdayIsWorking { get; set; }
        public int BranchID { get; set; }
    }
}
