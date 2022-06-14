using RabbitMQ.Client;
using System;
using System.Text;

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
                channel.QueueDeclare(queue: "Hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                string helloWorld = "Hello World";
                var body = Encoding.UTF8.GetBytes(helloWorld);

                for (int i = 0; i < 10000; i++)
                {
                    channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);

                    Console.WriteLine("Sent message {0} - {1} time", helloWorld, i  );
                }
                
            }

            Console.WriteLine("Press { enter } to exit");
            Console.ReadLine();
        }
    }
}
