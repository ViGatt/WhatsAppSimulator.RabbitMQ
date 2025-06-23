using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;
using WhatsAppSimulator.RabbitMQ.Models;
using WhatsAppSimulator.Consumer.RabbitMQ;

namespace WhatsAppSimulator.Consumer.Services
{
    public class MessageConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly HttpClient _httpClient;
        private readonly string _groupName;
        private AsyncEventingBasicConsumer _consumer;

        public MessageConsumer(string groupName)
        {
            _groupName = groupName;
            _httpClient = new HttpClient();
            _consumer = null;

            try
            {
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(RabbitMQConfig.ConnectionString),
                    DispatchConsumersAsync = true
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                SetupQueueAndExchange();
            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine($"❌ Falha na conexão com RabbitMQ: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro inesperado: {ex.Message}");
                throw;
            }
        }

        private void SetupQueueAndExchange()
        {
            try
            {
                _channel.ExchangeDeclare(
                    exchange: RabbitMQConfig.ExchangeName,
                    type: ExchangeType.Fanout,
                    durable: true);

                var queueName = GetQueueName();
                var dlqName = GetDLQName();

                var args = new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "" },
                    { "x-dead-letter-routing-key", dlqName }
                };

                _channel.QueueDeclare(
                    queue: dlqName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: args);

                _channel.QueueBind(
                    queue: queueName,
                    exchange: RabbitMQConfig.ExchangeName,
                    routingKey: "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao configurar filas: {ex.Message}");
                throw;
            }
        }

        public void StartConsuming()
        {
            var queueName = GetQueueName();
            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.Received += async (model, ea) =>
            {
                try
                {
                    var message = JsonSerializer.Deserialize<Message>(
                        Encoding.UTF8.GetString(ea.Body.ToArray()));

                    Console.WriteLine($"📩 Mensagem recebida no grupo {_groupName}: {message.Text}");

                    if (await ProcessMessage(message))
                    {
                        _channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine($"✔️ Mensagem processada com sucesso");
                    }
                    else
                    {
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        Console.WriteLine($"⚠️ Mensagem movida para DLQ");
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"❌ ERRO: Mensagem inválida - {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ ERRO: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: _consumer);

            Console.WriteLine($"👂 Ouvindo mensagens para o grupo {_groupName}...");
        }

        private async Task<bool> ProcessMessage(Message message)
        {
            try
            {
                using var content = new StringContent(
                    JsonSerializer.Serialize(message),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    "http://localhost:5000/mensagem/receber",
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    await NotifyError(message, $"HTTP {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                await NotifyError(message, ex.Message);
                return false;
            }
        }

        private async Task NotifyError(Message message, string error)
        {
            try
            {
                using var content = new StringContent(
                    JsonSerializer.Serialize(new FailedMessage
                    {
                        Sender = message.Sender,
                        Group = message.Group,
                        Text = message.Text,
                        SentTime = message.SentTime,
                        Error = error
                    }),
                    Encoding.UTF8,
                    "application/json");

                await _httpClient.PostAsync(
                    "http://localhost:5000/mensagem/erro",
                    content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Falha ao notificar erro: {ex.Message}");
            }
        }

        private string GetQueueName() => _groupName.ToLower() switch
        {
            "a" => RabbitMQConfig.GroupAQueue,
            "b" => RabbitMQConfig.GroupBQueue,
            "c" => RabbitMQConfig.GroupCQueue,
            _ => throw new ArgumentException("Grupo inválido")
        };

        private string GetDLQName() => _groupName.ToLower() switch
        {
            "a" => RabbitMQConfig.GroupADLQ,
            "b" => RabbitMQConfig.GroupBDLQ,
            "c" => RabbitMQConfig.GroupCDLQ,
            _ => throw new ArgumentException("Grupo inválido")
        };

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                _httpClient?.Dispose();
                _consumer = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erro ao liberar recursos: {ex.Message}");
            }
        }
    }
}