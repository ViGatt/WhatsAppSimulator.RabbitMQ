using WhatsAppSimulator.Consumer.Services;

Console.WriteLine("WhatsApp Simulator Consumer");
Console.WriteLine("Digite o grupo para consumir (a, b ou c)");
var group = Console.ReadLine()?.ToLower();

if (group != "a" && group != "b" && group != "c")
{
    Console.WriteLine("Grupo inválido. Use a, b ou c");
    return;
}

var consumer = new MessageConsumer(group);
consumer.StartConsuming();

Console.WriteLine("Pressione qualquer tecla para sair");
Console.ReadKey();

consumer.Dispose();