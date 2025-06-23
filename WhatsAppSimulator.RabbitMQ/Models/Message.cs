namespace WhatsAppSimulator.RabbitMQ.Models
{
    public class Message
    {
        public int Id { get; set; }  // Id para banco e controle
        public string Sender { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime SentTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = string.Empty;  // Ex: Pending, Error, Sent...
    }

    public class FailedMessage : Message
    {
        public string Error { get; set; } = string.Empty;
    }
}