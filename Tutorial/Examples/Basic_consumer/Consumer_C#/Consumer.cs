using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Create connection.
var factory = new ConnectionFactory{ HostName = "localhost"};

// Instantiate the connection and one channel.
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declare a queue called Transforms.
channel.QueueDeclare(
    queue : "Transforms", 
    durable: false, 
    exclusive: false, 
    autoDelete: false);

// Declare consumer.
var consumer = new EventingBasicConsumer(channel);

// Subscribe to the Received event.
consumer.Received += (model, ea) =>{
    // Get the body of the message as byte array, decode to string and print it.
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message received: {message}");
};

// Start consuming messages from Transforms.
channel.BasicConsume(queue: "Transforms", autoAck: true, consumer: consumer);
Console.WriteLine("Started consuming");


// Wait for a keypress to exit.
Console.ReadKey();