using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
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
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var correlationId = ea.BasicProperties.CorrelationId;
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] Received {message} - CorrelationId {correlationId}");

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: "test.queue",
                                    autoAck: false,
                                    consumer: consumer);
                
                Thread.Sleep(10000);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
