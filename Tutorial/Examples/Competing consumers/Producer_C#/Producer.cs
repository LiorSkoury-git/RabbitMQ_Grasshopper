using System;
using System.Threading.Tasks;
using System.Text;
using RabbitMQ.Client;

// Create connection.
var factory = new ConnectionFactory{ HostName = "localhost"};

// Instantiate the connection and one channel.
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declare a queue called CCQueue.
channel.QueueDeclare(
    queue : "CCQueue", 
    durable: false, 
    exclusive: false, 
    autoDelete: false,
    arguments: null);

// Dset a variable for the id of the message to send.
int messageId = 1;

// instantiate a random number genetaror.
var random = new Random();

while (true)
{
    // Declare message to produce and encode it as bytes.
    var message = $"This is message No. {messageId}";
    var body = Encoding.UTF8.GetBytes(message);

    // Start publishing to CCQueue.
    channel.BasicPublish("", "CCQueue", null, body);

    // Print the published message to console.
    Console.WriteLine($"Send message: {message}");

    var waitTime = random.Next(1, 5);

    Task.Delay(TimeSpan.FromSeconds(waitTime)).Wait();

    messageId++;
}

