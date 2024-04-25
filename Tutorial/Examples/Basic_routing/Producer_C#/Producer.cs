using System;
using System.Text;
using RabbitMQ.Client;

class Program{
    static void Main(string[] args){
        //Variable declaration.
        int x, y, l;

        // Create connection. When connecting to a real server, localhost should be replaced by the server´s address.
        var factory = new ConnectionFactory{ HostName = "localhost"};

        // Instantiate the connection and one channel.
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare an exchange called routing.
        channel.ExchangeDeclare(exchange: "routing", type: ExchangeType.Direct);

        // Instantiate a random number generator.
        Random random = new Random();

        try{
            x = int.Parse(args[0]);
            y = int.Parse(args[1]);
            l = int.Parse(args[2]);
        }
        catch{
            x = random.Next(1, 6);
            y = random.Next(1, 6);
            l = random.Next(1, 6);
            Console.WriteLine("Invalid arguments. Random values asigned");

        }
        

        // Declare message to produce and encode it as bytes.
        var message = string.Format("{0},{1},{2}", x, y, l);
        var encodedMessage = Encoding.UTF8.GetBytes(message);

        // Select key to use for routing the message.
        string[] keys = {"square", "circle", "shape"}; 
        string key = keys[random.Next(0, keys.Length)];

        // Start publishing to the routing exchange.
        channel.BasicPublish(exchange: "routing", routingKey: key, null, encodedMessage);

        // Print the published message to console.
        Console.WriteLine($"Sent message: {message} with the key {key}");
    }
}