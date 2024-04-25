using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program{
    static void Main(string[] args){
        // Create connection. When connecting to a real server, localhost should be replaced by the server´s address.
        var factory = new ConnectionFactory{ HostName = "localhost"};

        // Instantiate the connection and one channel.
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare an exchange.
        channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);

        // Declare a queue and bind it to the exchange.
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName, exchange: "pubsub", routingKey: "");

        // Declare consumer.
        var consumer = new EventingBasicConsumer(channel);

        // Subscribe to the Received event.
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[Subscriber 2] Recieved new message: {message}");
        };


        // Start consuming messages without auto aknowledging received messages.
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        Console.WriteLine("[Subscriber 2] Started consuming");

        // Wait for a keypress to exit.
        Console.ReadKey();
    }
}