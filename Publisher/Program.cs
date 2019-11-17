using System;
using System.Text;
using RabbitMQ.Client;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            { 
                HostName = "localhost",
                Port = 5672,
                VirtualHost = "dev",
                UserName = "jorge",
                Password = "senha",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(3)
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ConfirmSelect();

                string correlationId = Guid.NewGuid().ToString();

                var messageProperties = channel.CreateBasicProperties();
                messageProperties.Persistent = true;
                messageProperties.CorrelationId = correlationId;
                messageProperties.ContentType = "application/json";

                string message = @"{""message"":""Hello World!""}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "test.exchange",
                    routingKey: "",
                    basicProperties: messageProperties,
                    body: body
                );

                channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));

                Console.WriteLine($" [x] Sent {message} - CorrelationId {correlationId}");
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
