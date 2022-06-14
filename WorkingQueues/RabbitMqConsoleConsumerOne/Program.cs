using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMqConsoleConsumerOne
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
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received IN CONSUMER ONE {0}", message);

                    var randomNumberGenerated = new Random().Next(0, 10);
                    
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    Thread.Sleep(500);
                };

                channel.BasicAcks += OnNack;

                channel.BasicConsume(queue: "task_queue",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }


        private static void OnNack(object sender, BasicAckEventArgs e)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(e.DeliveryTag + "Was rejected");
            Console.WriteLine("----------------------------------------------");
            Console.BackgroundColor = ConsoleColor.White;
        }
    }
}
