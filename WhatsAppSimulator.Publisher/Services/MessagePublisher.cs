using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using WhatsAppSimulator.RabbitMQ.Models;
using WhatsAppSimulator.Publisher.RabbitMQ;

namespace WhatsAppSimulator.Publisher.Services
{
    public class MessagePublisher : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessagePublisher()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(RabbitMQConfig.ConnectionString),
                    ClientProvidedName = "WhatsApp.Publisher" // Nome identificador
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(
                    exchange: RabbitMQConfig.ExchangeName,
                    type: ExchangeType.Fanout,
                    durable: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao conectar: {ex.Message}");
                throw;
            }
        }

        public void PublishMessage(Message message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                _channel.BasicPublish(
                    exchange: RabbitMQConfig.ExchangeName,
                    routingKey: RabbitMQConfig.RoutingKey,
                    basicProperties: null,
                    body: body);

                Console.WriteLine($"✅ Mensagem enviada para o grupo {message.Group}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Falha ao publicar: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}