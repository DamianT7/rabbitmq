using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace LHC_Payment_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneralController : ControllerBase
    {
        private readonly ILogger<GeneralController> _logger;

        public GeneralController(ILogger<GeneralController> logger)
        {
            _logger = logger;
        }

        #region ~~ Redirects to Swagger ~~

        [HttpGet]
        [Route("/")]
        public RedirectResult Default()
        {
            return Redirect("/swagger");
        }

        [HttpGet]
        [Route("/api")]
        public RedirectResult Api()
        {
            return Redirect("/swagger/v1/swagger.json");
        }

        #endregion ~~ Redirects to Swagger ~~

        #region ~~ Ping ~~

        [HttpGet]
        [Route("/api/ping")]
        public string Ping()
        {
            return "pong";
        }

        #endregion ~~ Ping ~~

        #region ~~ RabbitMQ ~~

        [HttpGet]
        [Route("/api/sendmq")]
        public IActionResult SendMQ()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = "Hi there, I'm Damian!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            return Ok();
        }


        [HttpGet]
        [Route("/api/receivemq")]
        public IActionResult ReceiveMQ()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "hello",
                                     autoAck: true,
                                     consumer: consumer);

            }

            return Ok();
        }

        #endregion ~~ RabbitMQ ~~
    }
}
