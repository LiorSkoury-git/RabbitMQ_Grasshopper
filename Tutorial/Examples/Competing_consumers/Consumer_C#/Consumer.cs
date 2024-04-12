using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

// Set prefectch count to 1 so the consumer only gets one message at a time.
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

// Declare consumer.
var consumer = new EventingBasicConsumer(channel);

// Create a random numebr generator. 
var random = new Random();

// Subscribe to the Received event.
consumer.Received += (model, ea) =>
{
    // Random time to simulate the dealay caused by processing the message.
    var delay = random.Next(1, 6);
    // Get the body of the message as byte array, decode to string and print it.
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message received: {message}");
    // Simulate the message proceesing delay.
    Task.Delay( TimeSpan.FromSeconds(delay) ).Wait();
    // Acknowledge the processed message.
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};


// Start consuming messages from CCQueue without auto aknowledging received messages.
channel.BasicConsume(queue: "CCQueue", autoAck: false, consumer: consumer);
Console.WriteLine("Consuming");

// Wait for a keypress to exit.
Console.ReadKey();