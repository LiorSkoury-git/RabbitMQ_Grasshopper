﻿using System;
using System.Collections.Generic; 
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

class Program{
    static void Main(string[] args){
        //Variable declaration.
        int h;

        // Create connection. When connecting to a real server, localhost should be replaced by the server´s address.
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

        // Variable setup for message construction.
        int count = 0;
        int adder = 1;
        int[] A ={0,0,0};
        int[] B ={1,0,0};
        int[] vector;

        try{
            h = int.Parse(args[0]);
        }
        catch{
            h = 20;
            Console.WriteLine("Invalid argument. Default value asigned");

        }

        while (true)
        {   
            // Compose the message's body.
            if (count % 2 == 0){
                vector = A.Take(3).ToArray();
            }
            else{
                vector = B.Take(3).ToArray();
            }
            vector[1] = count;

            // Declare message to produce and encode it as bytes.
            string message = string.Join(",", vector);
            var body = Encoding.UTF8.GetBytes(message);

            // Start publishing to CCQueue.
            channel.BasicPublish("", "CCQueue", null, body);

            // Print the published message to console.
            Console.WriteLine($"Send message: {message}");

            // Add a delay that simulates processing time.
            Task.Delay(TimeSpan.FromSeconds(0.1)).Wait();

            // Update the id for the next message.
            if (count > h || count < 0){
                adder *= -1;
            }
            count += adder;
        }
    }
}