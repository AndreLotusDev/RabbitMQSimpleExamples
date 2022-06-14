using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace OneItemPerTimeConsumer
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

                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received IN CONSUMER ONE {0}", message);
                        Console.WriteLine("--------------------------------------------------");

                        var randomNumberGenerated = new Random().Next(0, 10);

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };

                    channel.BasicConsume(queue: "one_per_time",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}

