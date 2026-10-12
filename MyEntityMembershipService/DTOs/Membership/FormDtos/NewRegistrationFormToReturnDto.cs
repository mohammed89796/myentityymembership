namespace MyEntityMembershipService.DTOs.Membership.FormDtos
{
    public class NewRegistrationFormToReturnDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string User { get; set; }

        public DateOnly CreationDate { get; set; }


    }
}
