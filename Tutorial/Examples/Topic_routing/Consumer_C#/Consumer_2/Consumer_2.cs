using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program{
    static void Main(string[] args){
        // Create connection.  When connecting to a real server, localhost should be replaced by the server´s address.
        var factory = new ConnectionFactory{ HostName = "localhost"};

        // Instantiate the connection and one channel.
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare an exchange called topic.
        channel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic);

        // Declare a queue and bind it using the "square.area" key.
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName, exchange: "topic", routingKey: "square.area");

        // Declare consumer.
        var consumer = new EventingBasicConsumer(channel);

        // Subscribe to the Received event.
        consumer.Received += (model, ea) =>{
            // Get the body of the message as byte array, decode to string and print it.
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[Area consumer] Message received: {message}");
        };

        // Start consuming messages.
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        Console.WriteLine("[Area consumer] Started consuming");

        // Wait for a keypress to exit.
        Console.ReadKey();
    }
}