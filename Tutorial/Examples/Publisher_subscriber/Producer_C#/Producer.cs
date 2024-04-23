using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

class Program{
    static void Main(string[] args){
        //Variable declaration.
        int x, y, z, d;

        // Create connection.
        var factory = new ConnectionFactory{ HostName = "localhost"};

        // Instantiate the connection and one channel.
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);

        try{
            x = int.Parse(args[0]);
            y = int.Parse(args[1]);
            z = int.Parse(args[2]);
            d = int.Parse(args[3]);
        }
        catch{
            // Instantiate a random number generator
            Random random = new Random();
            x = random.Next(1, 6);
            y = random.Next(1, 6);
            z = random.Next(1, 6);
            d = random.Next(2, 5);
            Console.WriteLine("Invalid arguments. Random values asigned");
        }

        // Declare message to produce and encode it as bytes
        var message = string.Format("{0},{1},{2},{3}", x, y, z, d);
        var encodedMessage = Encoding.UTF8.GetBytes(message);


        // Publish to the pubsub exchange.
        channel.BasicPublish(exchange: "pubsub", "", null, encodedMessage);
        Console.WriteLine($"Sent message: {message}");
    }
}