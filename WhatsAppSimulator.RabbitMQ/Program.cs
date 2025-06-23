using Microsoft.EntityFrameworkCore;
using WhatsAppSimulator.RabbitMQ.Models; // Ajuste se seu DbContext estiver em outro namespace

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte ao appsettings.json (geralmente já vem por padrão, mas não faz mal)
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Pega a string de conexão do SQL Server (deve estar em appsettings.json dentro de "ConnectionStrings")
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o DbContext com a connection string do SQL Server
builder.Services.AddDbContext<WhatsAppSimulatorDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();