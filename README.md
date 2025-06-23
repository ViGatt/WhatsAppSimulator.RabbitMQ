# WhatsAppSimulator.RabbitMQ

Projeto desenvolvido como parte de um desafio proposto na disciplina de Computação Distribuída.

## 📌 Objetivo

Implementar um sistema de mensagens assíncronas utilizando **RabbitMQ com suporte a DLQ (Dead Letter Queue)**.

O sistema simula o envio de mensagens similar ao WhatsApp em grupo, com publishers, consumidores e gerenciamento de falhas com armazenamento em banco de dados.

## 🎯 Desafio proposto

- Configurar uma fila com **Dead Letter Queue (DLQ)**.
- Implementar o fluxo de publicação e consumo de mensagens.
- Caso ocorra falha no processamento (ex: erro de deserialização, falha no banco de dados), a mensagem deve ser movida automaticamente para a **DLQ**.
- Permitir o monitoramento e o reprocessamento manual ou automático dessas mensagens falhadas.

## 🧩 Estrutura do Projeto

- `WhatsAppSimulator.Publisher` – Responsável por publicar mensagens no RabbitMQ.
- `WhatsAppSimulator.Consumer` – Consome mensagens e simula o processamento.
- `WhatsAppSimulator.Common` – Contém modelos e contratos compartilhados.
- `WhatsAppSimulator.RabbitMQ` – Configurações de RabbitMQ, exchanges, filas e Dead Letter Queue.
- `WhatsAppSimulator.RabbitMQ.sln` – Arquivo de solução para o Visual Studio.

## 🛠️ Tecnologias Utilizadas

- C# / .NET
- RabbitMQ
- Entity Framework Core
- SQL Server (para armazenamento das mensagens)
- CloudAMQP (como broker RabbitMQ na nuvem)
- Visual Studio 

## 💾 Banco de Dados

As mensagens recebidas são **armazenadas em um banco de dados SQL Server**.  
A persistência é feita por meio do **Entity Framework Core**, garantindo rastreabilidade e controle das mensagens processadas com sucesso ou erro.

## ⚙️ Como executar

1. Clone o repositório:

   ```bash
   git clone https://github.com/seu-usuario/WhatsAppSimulator.RabbitMQ.git
   cd WhatsAppSimulator.RabbitMQ
Configure os seguintes serviços:

- `RabbitMQ (via CloudAMQP)`
- `SQL Server local ou remoto`
- `Ajuste as connection strings nos arquivos de configuração (appsettings.json) dos projetos Publisher e Consumer.`

## Execute os projetos na seguinte ordem:`
- `Inicie o serviço do RabbitMQ.`
- `Execute o projeto Publisher.`
- `Execute o projeto Consumer.`

## 📨 Dead Letter Queue (DLQ)  

- A fila principal do RabbitMQ está configurada com a propriedade x-dead-letter-exchange, que redireciona mensagens com erro para a DLQ.
- Quando o Consumer encontra um erro (por exemplo, uma falha de deserialização ou exceção no banco de dados), ele rejeita a mensagem sem requeue, movendo-a automaticamente para a DLQ.
- A fila de DLQ pode ser monitorada diretamente pelo painel de administração do RabbitMQ.

## 🧪 Tipos de falhas simuladas
- Mensagens inválidas que causam falha na desserialização.
- Erros intencionais de conexão com o banco de dados.
- Mensagens com formato inesperado.
