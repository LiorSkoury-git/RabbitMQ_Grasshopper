using System;
using System.Text;
using RabbitMQ.Client;

class Program{
    static void Main(string[] args){
        //Variable declaration.
        int x, y, z;

        // Create connection. When connecting to a real server, localhost should be replaced by the server´s address.
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

        
        try{
            x = int.Parse(args[0]);
            y = int.Parse(args[1]);
            z = int.Parse(args[2]);
        }
        catch{
            // Instantiate a random number generator.
            Random random = new Random();
            x = random.Next(1, 6);
            y = random.Next(1, 6);
            z = random.Next(1, 6);
            Console.WriteLine("Invalid arguments. Random values asigned");
        }
        
        // Declare message to produce and encode it as bytes.
        var message = string.Format("{0},{1},{2}", x, y, z);
        var encodedMessage = Encoding.UTF8.GetBytes(message);

        // Start publishing to Transforms.
        channel.BasicPublish("", "Transforms", null, encodedMessage);

        // Print the published message to console.
        Console.WriteLine($"Sent message: {message}");
    }
}