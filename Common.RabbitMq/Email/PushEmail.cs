namespace Common.RabbitMq.Email
{
    public class PushEmail
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public List<EmailAttachmentDto>? Attachments { get; set; } = [];

    }
}
