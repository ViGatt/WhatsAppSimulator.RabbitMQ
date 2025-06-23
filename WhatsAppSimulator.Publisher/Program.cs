using WhatsAppSimulator.Publisher.Services;
using WhatsAppSimulator.RabbitMQ.Models;

var publisher = new MessagePublisher();

Console.WriteLine("WhatsApp Simulator Publisher");
Console.WriteLine("Digite 'exit' para sair");
Console.WriteLine("Formato: <grupo>|<mensagem>");
Console.WriteLine("Exemplo: a|Ol� pessoal do grupo A!");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();

    if (input?.ToLower() == "exit")
        break;

    try
    {
        var parts = input.Split('|');
        if (parts.Length != 2)
        {
            Console.WriteLine("Formato inv�lido. Use: <grupo>|<mensagem>");
            continue;
        }

        var group = parts[0].Trim().ToLower();
        var text = parts[1].Trim();

        if (!new[] { "a", "b", "c" }.Contains(group))
        {
            Console.WriteLine("Grupo inv�lido. Use a, b ou c");
            continue;
        }

        var message = new Message
        {
            Sender = "user_" + Guid.NewGuid().ToString()[..4],
            Group = group,
            Text = text
        };

        publisher.PublishMessage(message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}

publisher.Dispose();