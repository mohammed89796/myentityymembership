namespace Common.RabbitMq.Premission
{
    public class PremissionRegisterDto
    {
        public string Service { get; set; }
        public string Entity { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string Method { get; set; } = default!;
    }
}
