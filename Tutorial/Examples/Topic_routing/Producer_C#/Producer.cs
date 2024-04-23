using System;
using System.Text;
using RabbitMQ.Client;

class Program{
    static void Main(string[] args){
        //Variable declaration.
        int l;

        // Create connection.
        var factory = new ConnectionFactory{ HostName = "localhost"};

        // Instantiate the connection and one channel.
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare an exchange called routing.
        channel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic);

        // Instantiate a random number generator
        Random random = new Random();


        try{
            l = int.Parse(args[0]);
        }
        catch{
            l = random.Next(1, 6);
            Console.WriteLine("Invalid arguments. Random value asigned");

        }

        // Declare message to produce and encode it as bytes
        var message = string.Format("{0}", l);
        var encodedMessage = Encoding.UTF8.GetBytes(message);

        // Select key to use for topic the message.
        string[] keys = {"square.area", "square.perimeter"}; 
        string key = keys[random.Next(0, keys.Length)];

        // Start publishing to the exchange.
        channel.BasicPublish(exchange: "topic", routingKey: key, null, encodedMessage);

        // Print the published message to console.
        Console.WriteLine($"Sent message: {message} with the key {key}");
    }
}