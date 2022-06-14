using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

              

                var size = NamesGenerator.Constants.firstNames.Length - 1;
                var random = new Random();

                for (int i = 0; i < 10000; i++)
                {
                    var rg = NamesGenerator.Constants.firstNames[random.Next(0, size)];
                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(rg);
                    channel.BasicPublish(exchange: "logs",
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);
                    Console.WriteLine(" [x] Sent {0}", rg);

                    Thread.Sleep(1000);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0)
                   ? string.Join(" ", args)
                   : "info: Hello World!");
        }
    }
}
