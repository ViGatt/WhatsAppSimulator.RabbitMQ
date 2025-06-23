using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WhatsAppSimulator.RabbitMQ.Models; // Ajuste o namespace da sua Model

namespace WhatsAppSimulator.RabbitMQ.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly WhatsAppSimulatorDbContext _context;

        public MessageController(WhatsAppSimulatorDbContext context)
        {
            _context = context;
        }

        [HttpPost("receber")]
        public IActionResult Receber([FromBody] Message message)
        {
            message.Status = "Pending";
            _context.Messages.Add(message);
            _context.SaveChanges();
            return Ok(new { message = "Mensagem recebida com sucesso", id = message.Id });
        }

        [HttpPost("erro")]
        public IActionResult MarcarErro([FromBody] int messageId)
        {
            var msg = _context.Messages.FirstOrDefault(m => m.Id == messageId);
            if (msg == null) return NotFound("Mensagem não encontrada");

            msg.Status = "Error";
            _context.SaveChanges();
            return Ok("Mensagem marcada como erro.");
        }

        [HttpPost("reenvio")]
        public IActionResult ReenviarErro()
        {
            var mensagensErro = _context.Messages
                .Where(m => m.Status == "Error")
                .ToList();

            foreach (var msg in mensagensErro)
            {
                msg.Status = "Pending"; // Marca novamente para reenviar
            }

            _context.SaveChanges();
            return Ok(new { quantidade = mensagensErro.Count, mensagem = "Mensagens com erro marcadas para reenvio." });
        }
    }
}