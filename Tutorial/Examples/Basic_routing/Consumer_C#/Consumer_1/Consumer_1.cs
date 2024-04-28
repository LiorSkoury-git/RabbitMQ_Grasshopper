using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program{
    static void Main(string[] args){
        // Create connection. When connecting to a real server, localhost should be replaced by the server�s address.
        var factory = new ConnectionFactory{ HostName = "localhost"};

        // Instantiate the connection and one channel.
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare an exchange called routing.
        channel.ExchangeDeclare(exchange: "routing", type: ExchangeType.Direct);

        // Declare a queue and bind it using the "circle" and "shape" keys.
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName, exchange: "routing", routingKey: "circle");
        channel.QueueBind(queue: queueName, exchange: "routing", routingKey: "shape");

        // Declare consumer.
        var consumer = new EventingBasicConsumer(channel);

        // Subscribe to the Received event.
        consumer.Received += (model, ea) =>{
            // Get the body of the message as byte array, decode to string and print it.
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[Circle consumer] Message received: {message}");
        };

        // Start consuming messages.
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        Console.WriteLine("[Circle consumer] Started consuming");

        // Wait for a keypress to exit.
        Console.ReadKey();
        Console.WriteLine("Loop stopped by user");
        connection.Close();
    }
}