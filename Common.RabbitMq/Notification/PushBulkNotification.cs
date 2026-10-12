namespace Common.RabbitMq.Notification
{
    public class PushBulkNotification
    {
        public List<string> UserId { get; set; }
        public string? ImageUrl { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
    }
}

