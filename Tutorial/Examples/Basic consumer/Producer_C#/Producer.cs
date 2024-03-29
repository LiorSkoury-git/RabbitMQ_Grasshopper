using System;
using System.Text;
using RabbitMQ.Client;

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

// Declare message to produce and encode it as bytes
var message = """{"Rhino.Geometry.Transform": "R0=(0.707106781186548,-0.707106781186547,0,1.4142135623731), R1=(0.707106781186547,0.707106781186548,0,1.41421356237309), R2=(0,0,1,0), R3=(0,0,0,1)"}""";
var encodedMessage = Encoding.UTF8.GetBytes(message);

// Start publishing to Transforms.
channel.BasicPublish("", "Transforms", null, encodedMessage);

// Print the published message to console.
Console.WriteLine($"Published message: {message}");