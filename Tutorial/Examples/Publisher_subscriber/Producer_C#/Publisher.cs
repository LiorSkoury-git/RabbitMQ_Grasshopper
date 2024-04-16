using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

// Create connection.
var factory = new ConnectionFactory{ HostName = "localhost"};

// Instantiate the connection and one channel.
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);

// Instantiate a random number generator
Random random = new Random();
// Variable setup for message construction.
int x = random.Next(1, 6);
int y = random.Next(1, 6);
int z = random.Next(1, 6);
int d = random.Next(2, 5);


// Declare message to produce and encode it as bytes
var message = string.Format("{0},{1},{2},{3}", x, y, z, d);
var encodedMessage = Encoding.UTF8.GetBytes(message);


// Publish to the pubsub exchange.
channel.BasicPublish(exchange: "pubsub", "", null, encodedMessage);
Console.WriteLine($"Sent message: {message}");