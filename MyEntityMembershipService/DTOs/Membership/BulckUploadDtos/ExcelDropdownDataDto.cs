namespace MyEntityMembershipService.DTOs.Membership.BulckUploadDtos
{
    public class ExcelDropdownDataDto
    {
        public string RangeName { get; set; }                 // Named range (e.g., "AreaList")
        public int SourceStartColumn { get; set; }            // Column to place values in "Dropdowns" sheet (e.g., 1 = A)
        public string SourceHeader { get; set; }              // Header title (e.g., "AreaId")
        public List<KeyValuePair<int, string>> SourceData { get; set; } // Id and Name
        public int TargetColumnInMainSheet { get; set; }      // Which column gets the dropdown (e.g., 4 = D)
    }
}
