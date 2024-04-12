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

// Instantiate a random number generator
Random random = new Random();
int x = random.Next(1, 6);
int y = random.Next(1, 6);
int z = random.Next(1, 6);

// Declare message to produce and encode it as bytes
var message = string.Format("{0},{1},{2}", x, y, z);
var encodedMessage = Encoding.UTF8.GetBytes(message);

// Start publishing to Transforms.
channel.BasicPublish("", "Transforms", null, encodedMessage);

// Print the published message to console.
Console.WriteLine($"Sent message: {message}");