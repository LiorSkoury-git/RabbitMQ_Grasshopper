using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Create connection.
var factory = new ConnectionFactory() { HostName = "localhost" };

// Instantiate the connection and one channel.
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declare the queue for replies.
var replyQueue = channel.QueueDeclare("", exclusive:true);

// Declare the queue for requests.
channel.QueueDeclare("request-queue", exclusive:false);


// Declare consumer.
var consumer = new EventingBasicConsumer(channel);

// Subscribe to the Received event.
consumer.Received += (model, ea) =>
{   
    // Get the body of the reply message as byte array, decode to string and print it.
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received reply: {message}");
};

// Start consuming messages from the reply queue.
channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);

// Set the replies to go to the reply queue.
var properties = channel.CreateBasicProperties();
properties.ReplyTo = replyQueue.QueueName;

// Get a unique id to relate replies qith requests.
properties.CorrelationId = Guid.NewGuid().ToString();



// Instantiate a random number generator
Random random = new Random();
int diam = random.Next(1, 6);

// Compose and encode the request message.
var message = diam.ToString();
var body = Encoding.UTF8.GetBytes(message);

Console.WriteLine($"Sent area request {properties.CorrelationId} for diameter {message}");

// Send the generated request message.
channel.BasicPublish("", "request-queue", properties, body);

// Wait for a keypress to exit.
Console.ReadKey();