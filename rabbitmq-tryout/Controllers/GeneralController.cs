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
        public IActionResult SendMQ(string queue, string message)
        {
            try
            {
                if (null == queue || "" == queue ||
                    null == message || "" == message)
                    return BadRequest("Provide a message and queue name in the params");

                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: queue,
                                         basicProperties: null,
                                         body: body);
                    _logger.LogInformation(" [x] Sent {0}", message);
                }

                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError("Somthing went wrong: " + e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("/api/receivemq")]
        public IActionResult ReceiveMQ(string queue)
        {
            try
            {
                if (null == queue || "" == queue)
                    return BadRequest("Provide a queue name in the params");

                var message = "";
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        message = Encoding.UTF8.GetString(body);
                        _logger.LogInformation(" [x] Received {0}", message);
                    };

                    channel.BasicConsume(queue: queue,
                                         autoAck: true,
                                         consumer: consumer);
                }
                if(message == "")
                {
                    _logger.LogError(" [x] Nothing in the queue");
                    return BadRequest("Nothing in the queue");
                }

                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError("Somthing went wrong: " + e.Message);
                return StatusCode(500);
            }
        }

        #endregion ~~ RabbitMQ ~~
    }
}
