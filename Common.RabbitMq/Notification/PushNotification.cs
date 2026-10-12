namespace Common.RabbitMq.Notification
{
    public class PushNotification
    {
        public string UserId { get; set; }
        public string? ImageUrl { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
    }
}

