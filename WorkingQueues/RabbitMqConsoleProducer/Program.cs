using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace RabbitMqConsoleProducer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                var nameGenerator = new NamesGenerator();

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                for (int i = 0; i < 10000; i++)
                {
                    string randomMessage = nameGenerator.GetRandomName();
                    var body = Encoding.UTF8.GetBytes(randomMessage);
                    channel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: properties, body: body);

                    Console.WriteLine("Sent message {0} - {1} time", randomMessage, i);

                    //Thread.Sleep(10000);
                }

            }

            Console.WriteLine("Press { enter } to exit");
            Console.ReadLine();
        }
    }
}
