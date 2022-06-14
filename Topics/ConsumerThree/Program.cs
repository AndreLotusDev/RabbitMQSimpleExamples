using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ConsumerThree
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
                    channel.ExchangeDeclare(exchange: "info_logs",
                                             type: "topic");

                    var queueName = channel.QueueDeclare("CONSUMER_THREE_LOGS");

                    channel.QueueBind(queue: "CONSUMER_THREE_LOGS",
                                          exchange: "info_logs",
                                          routingKey: "logs.*");

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine(" [x] Received '{0}':'{1}'",
                                          routingKey, message);
                    };
                    channel.BasicConsume(queue: queueName.QueueName,
                                         autoAck: true,
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

        public enum Severity
        {
            Low,
            Medium,
            High
        }
    }
}
