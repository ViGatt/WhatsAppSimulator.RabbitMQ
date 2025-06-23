// WhatsAppSimulator.Consumer/RabbitMQ/RabbitMQConfig.cs
namespace WhatsAppSimulator.Consumer.RabbitMQ
{
    public static class RabbitMQConfig
    {
        // Configurações de conexão
        public static string HostName = "jackal.rmq.cloudamqp.com";
        public static string VirtualHost = "jfxuukbs";
        public static string UserName = "jfxuukbs";
        public static string Password = "yXBk2OsAGvjMZm1cuS7tJsvBk0wiZHNZ";

        // Propriedade ConnectionString calculada
        public static string ConnectionString =>
            $"amqps://{UserName}:{Password}@{HostName}/{VirtualHost}";

        // Nomes do Exchange e Filas
        public static string ExchangeName = "whatsapp_fanout_exchange";

        public static string GroupAQueue = "fila.grupo.a";
        public static string GroupBQueue = "fila.grupo.b";
        public static string GroupCQueue = "fila.grupo.c";

        public static string GroupADLQ = "fila.grupo.a.dlq";
        public static string GroupBDLQ = "fila.grupo.b.dlq";
        public static string GroupCDLQ = "fila.grupo.c.dlq";
    }
}