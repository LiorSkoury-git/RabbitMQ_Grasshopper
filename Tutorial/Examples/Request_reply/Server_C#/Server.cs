using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program{
    static void Main(string[] args){
        //Variable declaration.
        float diameter;
        // Create connection. When connecting to a real server, localhost should be replaced by the server´s address.
        var factory = new ConnectionFactory() { HostName = "localhost" };

        // Instantiate the connection and one channel.
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declare the queue for requests.
        channel.QueueDeclare("request-queue", exclusive:false);

        // Declare consumer.
        var consumer = new EventingBasicConsumer(channel);

        // Subscribe to the Received event.
        consumer.Received += (model, ea) =>
        {   
            // Get the body of the request and decode it. 
            var req_body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(req_body);

            // Print received request.
            Console.WriteLine($"Received area request with id: {ea.BasicProperties.CorrelationId} for diameter {message}");

            // Get the body of the reply message as byte array, encode to string and print it.
            try{
                diameter = float.Parse(message);
            }
            catch{
                // Instantiate a random number generator
                Random random = new Random();
                diameter = random.Next(2, 5);
                Console.WriteLine("Invalid arguments. Random value asigned");
            }


            float area = AreaFromDiameter(diameter);
            var replyMessage = $"The requested area for {ea.BasicProperties.CorrelationId} is {area}";
            var body = Encoding.UTF8.GetBytes(replyMessage);
            Console.WriteLine(replyMessage);

            // Send the composed reply.
            channel.BasicPublish("", ea.BasicProperties.ReplyTo, null, body);
        };

        // Start consuming messages form the request-queue.
        channel.BasicConsume(queue: "request-queue", autoAck: true, consumer: consumer);

        // Wait for a keypress to exit.
        Console.ReadKey();
    }

    static float AreaFromDiameter(float diam)
    {
        // Calculate the area of a circle given a diameter.
        return (float)(Math.Pow(diam, 2)/4*Math.PI);
    }

}