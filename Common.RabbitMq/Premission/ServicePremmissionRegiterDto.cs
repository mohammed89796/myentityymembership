namespace Common.RabbitMq.Premission
{
    public class ServicePremmissionRegiterDto
    {
        public string ServiceName { get; set; } = default!;
        public List<PremissionRegisterDto> Permissions { get; set; } = new List<PremissionRegisterDto>();
    }
}
