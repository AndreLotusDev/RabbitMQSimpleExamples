using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace EmitLogPublisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs",
                                        type: "direct");

                var size = NamesGenerator.Constants.firstNames.Length - 1;
                var random = new Random();

                Array values = Enum.GetValues(typeof(Severity));

                for (int i = 0; i < 10_000; i++)
                {
                    var message = NamesGenerator.Constants.firstNames[random.Next(0, size)];

                    Severity severity = (Severity)values.GetValue(random.Next(0, values.Length));

                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "direct_logs",
                                         routingKey: "logs" + "." + severity.ToString(),
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);

                    Thread.Sleep(100);
                }             
                
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        public enum Severity
        {
            Low, 
            Medium,
            High
        }
    }
}
