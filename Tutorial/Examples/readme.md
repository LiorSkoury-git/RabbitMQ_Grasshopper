# EXAMPLE FILES
Each pattern has a folder with its name documenting the implementation in C#, Python, and Grasshopper. All of them follow a similar structure: The folder contains the Grasshopper files and two additional directories; One for the Python files, and one for the C# files.

Both the C# and Python files can be run from the terminal window either passing the arguments specified in this document or passing no arguments to use default values. **Make sure that the RabbitMQ server is already running when trying to run the example files.**

**python requirements - pika (pip install pika)**

**C# requirements - .NET 8 SDK (winget install dotnet-sdk-8)**

## Basic consumer

The [basic consumer](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Basic_consumer) pattern consists of two elements. The producer sends messages to a queue while a consumer retrieves and processes one after the other as soon as it has available resources.

### Producer

For this example, the producer publishes a message with three numbers. Each number corresponds to x, y, and z coordinates. They can be passed as arguments when calling the corresponding Python or C# scripts. When left empty, a random value between 1 and 5 will be assigned to each coordinate. 

In the *.gh* file, random coordinates can be generated with the Python component connected to the panel called *Message body*. To overrun them, simply disconnect the Python component and manually input the coordinates in the panel separated by commas. To send the message, set the boolean toggle for the *Run* input to ```True```.

### Consumer

In the case of the C# and Python examples, the consumer doesn´t take any arguments and, after being run from the terminal window, will print any received messages.

The *.gh* example file, on the other hand, will start consuming messages after the boolean toggle for the *Run* input is set to ```True```. When it receives a message, the three numbers will be used as the components of the vector to move a box and display it. An additional Python component is included to handle possible errors. For example, when no messages have been received yet.

## Competing consumers

The [competing consumers](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Competing_consumers) pattern behaves similarly to the basic consumer-producer pattern. The difference is that multiple consumers are retrieving messages from the queue. this allows for a setup in which any consumer with free resources can take the next message in the queue and process it to avoid a message buildup that would occur when a consumer can´t process the messages faster than they arrive in the queue.

### Producer
In this example file, the producer is constantly sending messages with 3D coordinates to the queue. However, the coordinates are determined programmatically instead of taken as an argument. For each message, they increase in the y-direction until a limit is reached and they decrease to 0 before starting to increase once more. 

This results in a bouncing behavior used for visualization in Grasshopper with the sent y-coordinates always oscillating between 0 and the limit set. The limit can be determined as an argument when calling the Python or C# Producer scripts from a terminal window or will take a random value between 15 and 19 if no argument is specified.

The *Producer.gh* file does not generate coordinates. Instead, it generates a String with the message *This is message No. i* where *i* is replaced by the number of the generated message.
To run this example, set the boolean toggle for the *Run* input to ```True``` and hit the play button on the trigger connected to the C# component that outputs to the *Message body* panel.

### Consumer
Both the C# and Python examples come with two consumer scripts that behave equally. When a message is received they print the message preceded by *[Consumer 1]* or *[Consumer 2]* depending on which script is run. To run them, simply call them using a terminal window.

To run the *.gh* file for this example, set the boolean toggle for the *Run* input to ```True```. A white line starting near the origin should move one of its endpoints in the y-direction after each message is received creating a zig-zag effect with the line going back and forth from y=0 to the y-limit set when running the producer. Two consumer components are set up in this example, each one controls the location of one of the line´s endpoints. **This file will only work with the *Producer.py* and *Producer.cs* example files, not the *Producer.gh* file because it is sending the strings mentioned before instead of coordinates**.

## Publisher-subscriber
Like the competing consumers pattern, the [publisher-subscriber](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Publisher_subscriber) has multiple consumers retrieving messages. However, in this case, the messages are not received by one consumer at a time. Instead, the exchange routes the same message to all subscribers, and they process it according to their specific functionality.

### Publisher
The C# and Python *Publisher* scripts can be run from the command line like the producer examples from the [competing consumers](#competing-consumers) pattern. 4 numbers can optionally be passed as arguments. The first three determine 3D coordinates for the center of a sphere while the last one determines its diameter.

The *Publisher.gh* has the same behavior. The numbers are randomly generated by a Python component plugged into the *Message body* panel that can be overrun by unplugging it from the component and manually inputting the numbers separated by commas. To run it, set the boolean toggle for the *Run* input to ```True```. 

### Subscriber
The Python and C# versions of the subscriber work in the same way as the consumers in the [competing consumers](#competing-consumers) pattern with the difference being that each subscriber will receive **all** the messages published by the publisher.

The *Subscriber.gh* file contains two subscriber components that will receive the published messages and process them to display spheres on the specified coordinates and with the specified diameter. Even though they receive the exact same information, the second of them will process it to get the mirrored output of the first one. Like in previous examples, set the boolean toggle for the *Run* input to ```True``` to run it, and one Python component is included to handle possible errors. 

## Request-reply
The [request-reply](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Request_reply) pattern works with a different structure than the previously covered patterns. Two elements interact in it: The Client and the Server. The main difference is that both can send and receive messages. The first one sends a request to the second one, which processes it and sends a corresponding response. To keep track of the relation between a reply and its corresponding request, a unique identifier is attached when sending a request so its reply can reference it.

### Client
For the example files, the Client sends a request with a number, and a unique ID number automatically generated. The number represents the diameter of a circle and the request asks the Server for its area. As in previous examples, the Python and C# files can be run from the command line and the number be passed as an argument or left empty to default to a random value. the *.gh* file will use the number in the panel named *Circle diameter to request area*.

### Server
Both the Python and C# files can be run from the terminal interface and will reply to a request from the client with the calculated area for the requested number. The *Server.gh* will, in addition, visualize the circle whose area was requested. 

Note that the Server component uses a [custom handler](#custom-handlers) to process the request data into the Grasshopper geometry for the corresponding circle.

## Basic routing
The [basic routing](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Basic_routing) pattern leverages a set of routing keys to bind queues from which consumers receive messages to an exchange that directs only relevant messages to the queue. This is done by checking the routing keys sent with the message and delivering it only to the queues that share at least one of those keys. 

### Producer
The example Python and C# files for the Producer will take three numbers as in previous examples. The first two represent the 2D coordinates for the center of a shape and, the last one, its characteristic dimension. When publishing a message, a random key will be used to route it to its proper consumer. The possible keys are *square*, *circle*, and *shape*.

In the *.gh* file, a Python component randomly assigns both the numbers for the message as well as the routing key.

### Consumer
Both the C# and Python examples have two *Consumer* files. Their behavior is similar, but one will receive only messages with the ```square``` routing key, and the other one messages with the ```circle``` key. In addition, both consumers will receive messages with the ```shape``` key. These scripts will then print the received message.

The *Consumer.gh* has also two consumers with the same behavior but will, additionally, create the corresponding geometry for visualization. As in other Grasshopper examples, Python components are used to handle errors.

## Topic routing
The [topic routing](https://github.com/LiorSkoury-git/RabbitMQ_Grasshopper/tree/Documentation/Tutorial/Examples/Topic_routing) pattern works in a similar way as the [basic routing](#basic-routing). The difference is that a special syntax is applied to routing keys. Instead of having any value, the words or segments in routing keys are separated using the ```.``` character. When designing a binding, the ```*``` and ```#``` characters can be used for pattern matching. 

The ```*``` character will match exactly one word or segment and the ```#``` character will match zero or more segments in the routing key, allowing for more flexibility than the previous pattern.

## Producer
The example files for this pattern behave in a similar way as its basic form. The only differences are in the available keys and the meaning of the input number. These are ```square.perimeter``` or ```square.area```, and the number will always represent the side of a square.

## Consumer
The Consumer example scripts will act in a similar manner to the basic pattern, but this time, three consumers are used. For the Python and C# files, one consumer will only receive messages with the ```square.perimeter``` key, one messages with the ```square.area``` key, and one messages using the ```square.*``` key, meaning that it will receive all messages using any key starting with ```square.```. They will then print the received message with the corresponding calculation.

The *Consumer.gh* file will have three *Consumer* components behaving in the same way but will, in addition to the calculation, visualize the corresponding square. 

## Custom handlers
The *Consumer*, *Server*, and *Client* components have a custom handler input as mentioned in the [Request-reply](#request-reply) pattern. These are C# scripts that specify custom behavior to handle incoming messages and follow the same general structure as illustrated in the component created for the example:


```csharp
// Include any necessary using statements for the proper functioning of the custom handler 
using RabbitMQ.Core;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
```
```csharp
private void RunScript(ref object circle){
    // Asign the custom handler to the component´s output.
    circle = costumHandler;
}
/// Custom additional code
// Declare and initialize the custom handler.
public RabbitMQ.Core.Handler costumHandler = new CostumHandler();

// Define the CustomHandler class.
public class CostumHandler:RabbitMQ.Core.Handler{
  // Inherited constructor.
  public CostumHandler() : base() { }

  // Override the parent´s HandleMessage method with custom implementation.
  public override List<object> HandleMessage(string body){
    // Deserialize the recieved message.
    double radius = JsonConvert.DeserializeObject<double>(body);

    // Process the received information.
    var circle = new Circle(new Point3d(0, 0, 0), radius);

    // The returned value most be list of objects.
    // Wrap the object(s) to return in a list.
    List < object > objects = new List<object>();
    objects.Add(circle);

    return objects;
  }
}

 ```
