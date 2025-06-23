namespace WhatsAppSimulator.Publisher.RabbitMQ
{
    public static class RabbitMQConfig
    {
        public static string ExchangeName = "whatsapp_fanout_exchange";
        public static string RoutingKey = "";

        public static string ConnectionString =
            "amqps://jfxuukbs:yXBk2OsAGvjMZm1cuS7tJsvBk0wiZHNZ@jackal.rmq.cloudamqp.com/jfxuukbs";
    }
}