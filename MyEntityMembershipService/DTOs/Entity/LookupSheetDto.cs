namespace MyEntityMembershipService.DTOs.Entity
{
    public class LookupSheetDto
    {
        public string RangeName { get; set; }      
        public string DisplayName { get; set; }     
        public int TargetColumnInMainSheet { get; set; }
        public List<LookupItem> Items { get; set; } = new();
    }

    public class LookupItem
    {
        public int Id { get; set; }
        public string Label { get; set; }
    }
}
