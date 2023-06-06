using System;
using System.Text;
using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;


namespace Api.Estudos.RabbitMQ
{
    class Program
    {
     
        static void Main(string[] args)
        {
            args = new String[12];
            args[0] = "http://google.com.br";
            args[1] = "source_port";// something integer value "1324"
            args[2] = "-h";
            args[3] = "hostname";
            args[4] = "-p";
            args[5] = "port";
            args[6] = "-l";
            args[7] = "log_file";
            args[8] = "-m";
            args[9] = "misuse_keyword";
            args[10] = "-t";
            args[11] = "misuse_threshold";


            var logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
                .CreateLogger();
            logger.Information(
                "Testando o envio de mensagens para uma Fila do RabbitMQ");
            if (args.Length < 3)
            {
                logger.Error(
                    "Informe ao menos 3 parametros: " +
                    "no primeiro a string de conexao com o RabbitMQ, " +
                    "no segundo a Fila/Queue a que recebera as mensagens, " +
                    "ja no terceito em diante as mensagens a serem " +
                    "enviadas a Queue do RabbitMQ...");
                return;
            }

            string connectionString = args[0];
            string queueName = args[1];

            logger.Information($"Queue = {queueName}");

            try
            {
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(connectionString)
                };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                for (int i = 2; i < args.Length; i++)
                {
                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName,
                                         basicProperties: null,
                                         body: Encoding.UTF8.GetBytes(args[i]));
                    logger.Information(
                        $"[Mensagem enviada] {args[i]}");
                }

                logger.Information("Concluido o envio de mensagens");
            }
            catch (Exception ex)
            {
                logger.Error($"Exceção: {ex.GetType().FullName} | " +
                             $"Mensagem: {ex.Message}");
            }
        }
    }
}
