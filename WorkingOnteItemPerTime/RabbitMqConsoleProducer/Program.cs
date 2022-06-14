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
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "one_per_time",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var nameGenerator = new NamesGenerator();

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    for (int i = 0; i < 10000; i++)
                    {
                        if (i % 10 == 0)
                        {
                            Thread.Sleep(5000);
                        }

                        string randomMessage = nameGenerator.GetRandomName();
                        var body = Encoding.UTF8.GetBytes(randomMessage);
                        channel.BasicPublish(exchange: "", routingKey: "one_per_time", basicProperties: properties, body: body);

                        Console.WriteLine("Sent message {0} - {1} time", randomMessage, i);
                    }

                }

                Console.WriteLine("Press { enter } to exit");
                Console.ReadLine();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
