using Microsoft.EntityFrameworkCore;
using WhatsAppSimulator.RabbitMQ.Models;

namespace WhatsAppSimulator.RabbitMQ.Models
{
    public class WhatsAppSimulatorDbContext : DbContext
    {
        public WhatsAppSimulatorDbContext(DbContextOptions<WhatsAppSimulatorDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
    }
}