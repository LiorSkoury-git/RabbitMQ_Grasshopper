using System;
using System.Text;
using RabbitMQ.Client;

// Create connection.
var factory = new ConnectionFactory{ HostName = "localhost"};

// Instantiate the connection and one channel.
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declare an exchange called routing.
channel.ExchangeDeclare(exchange: "routing", type: ExchangeType.Direct);

// Instantiate a random number generator
Random random = new Random();
int x = random.Next(1, 6);
int y = random.Next(1, 6);
int l = random.Next(1, 6);

// Declare message to produce and encode it as bytes
var message = string.Format("{0},{1},{2}", x, y, l);
var encodedMessage = Encoding.UTF8.GetBytes(message);

// Select key to use for routing the message.
string[] keys = {"square", "circle", "shape"}; 
string key = keys[random.Next(0, keys.Length)];

// Start publishing to the exchange.
channel.BasicPublish(exchange: "routing", routingKey: key, null, encodedMessage);

// Print the published message to console.
Console.WriteLine($"Sent message: {message} with the key {key}");