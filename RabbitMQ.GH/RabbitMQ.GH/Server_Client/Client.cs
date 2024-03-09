using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Grasshopper.Kernel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Core;
using RabbitMQ.GH.Properties;
using Rhino;
using Rhino.Geometry;

namespace RabbitMQ.GH.Server_Client
{
    /// <summary>
    /// Represents a RabbitMQ Client inheriting from the GH_component class.
    /// </summary>
    public class Client : GH_Component
    {
        #region Fields

        /// <summary>
        /// Represents the state of the timer that tracks message consumption time.
        /// </summary>
        private bool timerStarted;

        /// <summary>
        /// Represents the state of the last sent message.
        /// </summary>
        private bool messageSent;

        /// <summary>
        /// Represents the current message to process.
        /// </summary>
        private string currentMessage;

        /// <summary>
        /// Represents the last received messages.
        /// </summary>
        private List<object> lastReceivedMessage;

        /// <summary>
        /// Represents a TCP connection to a RabbitMQ broker.
        /// </summary>
        private Connection connection;

        /// <summary>
        /// Represents a message handler.
        /// </summary>
        private Handler handler;

        /// <summary>
        /// Represents a RabbitMQ consumer.
        /// </summary>
        private EventingBasicConsumer replyConsumer;

        #endregion Fields

        #region Constructor
        /// <summary>
        /// Default constructor. Invokes the base class constructor and then extends it by initializing the
        /// lastReceivedMessage, timerStarted, messageSent, and currentMessage fields.
        /// </summary>
        public Client()
          : base("Client", "Client",
              "A client object that send messages and wait for reply",
              "RabbitMQ", "Server-Client")
        {
            lastReceivedMessage = new List<object>() { "Nothing to read yet..." };
            timerStarted = false;
            messageSent = false;
            currentMessage = null;
        }

        #endregion Constructor

        /// <summary>
        /// Represents the exposure level of the component.
        /// </summary>
        /// /// <value>
        /// The value of 2 sets the Consumer component to be displayed in the first section of the Grasshopper toolbar.
        /// </value>
        public override GH_Exposure Exposure => (GH_Exposure)2;

        #region Methods

        /// <summary>
        /// Initializes a RabbitMQ.Core Connection.
        /// </summary>
        /// <param name="host">Server host name.</param>
        /// <param name="port">Server port number.</param>
        /// <param name="vHost">Virtual host name.</param>
        private void setConnection(string host, int port, string vHost)
        {
            // Handles null values of vHost.
            if (vHost == null) { vHost = "/"; }

            connection = new Connection();

            // Sets up the connection with authentication.
            if (RhinoDoc.ActiveDoc.RuntimeData.ContainsKey("RabbitMQ_USER") && RhinoDoc.ActiveDoc.RuntimeData.ContainsKey("RabbitMQ_PASS"))
            {
                connection.setFactory(host, port, (string)RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_USER"], (string)RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_PASS"], vHost);
            }
            // Sets up the connection without authentication.
            else
            {
                connection.setFactory(host, port, vHost);
            }

            // Handles connection errors.
            bool connect = connection.CreateConnection();
            if (!connect) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error connecting to RabbitMQ"); }
        }

        /// <summary>
        /// Sends a message to the exchange.
        /// </summary>
        /// <param name="queue">Queue name.</param>
        /// <param name="exchange">Exchange name.</param>
        /// <param name="exType">Exchange type.</param>
        /// <param name="message">Message to send.</param>
        private void sendMessage(string queue, string exchange, int exType, string message)
        {
            // If no queue name is provided, defaults to "".
            if (queue != "") { connection.setQueue(queue, Exclusive: false); }
            else { connection.setQueue(queue, Exclusive: true); }

            // If no exchange name is provided, defaults to "".
            if (exchange != "" && exchange != null)
            {
                connection.setExchange(exchange, exType);
            }
            else { exchange = ""; }

            // Sets an exclusive queue for server replies.
            var replyQueue = connection.channel.QueueDeclare("", exclusive: true);

            // Sets a consumer for server replies, subscribe it to the Received event and start consuming.
            replyConsumer = new EventingBasicConsumer(connection.channel);
            replyConsumer.Received += HandleReceivedMessage;
            connection.channel.BasicConsume(queue: replyQueue.QueueName, autoAck: false, consumer: replyConsumer);

            // Sets the reply queue and an Global unique id to relate the sent message with its corresponding reply.
            var properties = connection.channel.CreateBasicProperties();
            properties.ReplyTo = replyQueue.QueueName;
            properties.CorrelationId = Guid.NewGuid().ToString();

            // Encodes and sends the message to the exchange.
            var encodeMessage = Encoding.UTF8.GetBytes(message);
            connection.channel.BasicPublish(exchange, routingKey: queue, properties, encodeMessage);

            // Updates state variables.
            messageSent = true;
            timerStarted = true;
        }

        /// <summary>
        /// Handles a newly received message.
        /// </summary>
        /// <param name="sender">Sender of the message.</param>
        /// <param name="ea">Deliver event arguments.</param>
        private void HandleReceivedMessage(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                RhinoApp.WriteLine("Handling message...");
                // Acknowledges the message.
                connection.channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                // Gets the message body decoded as String.
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var result = handler.HandleMessage(body);
                // Updates state variables.
                lastReceivedMessage = result;
                RhinoApp.WriteLine("Message handled successfully.");
                ExpireSolution(true);
            }
            catch (Exception ex)
            {
                //Error handling.
                RhinoApp.WriteLine($"Exception in HandleMessageReceived: {ex.Message}");
                //ExpireSolution(true);
            }

            // Cancels the queue used for server replies.
            connection.channel.BasicCancel(consumerTag: replyConsumer.ConsumerTags[0]);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Start or stop consuming", GH_ParamAccess.item);
            pManager.AddTextParameter("Host", "H", "Host url name", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Port", "P", "Port number of the broker", GH_ParamAccess.item);
            pManager.AddTextParameter("Body", "B", "Message to send to the client", GH_ParamAccess.item);
            pManager.AddTextParameter("Queue", "Q", "Queue name for the client", GH_ParamAccess.item);
            pManager.AddTextParameter("Exchange", "Ex", "Exchange name for the client", GH_ParamAccess.item);
            pManager.AddIntegerParameter("ExchangeType", "Ex_T", "Exchange type: 0=Fanout,1=Direct,2=Topic", GH_ParamAccess.item);
            pManager.AddGenericParameter("Handler", "Ha", "Handler object to do somthing with the reply data", GH_ParamAccess.item);
            pManager.AddTextParameter("vHost", "VH", "Virtual host if needed", GH_ParamAccess.item);


            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Received", "R", "Last Recevied message", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Represents the state of the component. true if it is activated false otherwise.
            bool b = false;

            // RabbitMQ state variables.
            string host = null;
            int port = 5672;
            string queue = "";
            string exchange = null;
            int exType = 1;
            string message = null;
            handler = new Handler();
            string vHost = null;

            // Get data from the component´s inputs
            DA.GetData("Run", ref b);
            DA.GetData("Host", ref host);
            DA.GetData("Port", ref port);
            DA.GetData("Queue", ref queue);
            DA.GetData("Exchange", ref exchange);
            DA.GetData("ExchangeType", ref exType);
            DA.GetData("Body", ref message);
            DA.GetData("vHost", ref vHost);

            if (b)
            {

                if (currentMessage != message)
                {
                    currentMessage = message;
                    if (!timerStarted)
                    {
                        setConnection(host, port, vHost);
                    }
                    sendMessage(queue, exchange, exType, message);
                }
            }
            else
            {
                if (timerStarted)
                {
                    try
                    {
                        connection.closeConnection(); // Close the connection setup if not done yet
                    }
                    catch { RhinoApp.WriteLine("The connection is closed"); }


                    // Update state variables.
                    lastReceivedMessage = new List<object>() { "Turn on the Run to start consuming" };
                    timerStarted = false;
                    messageSent = false;
                    currentMessage = null;
                }
            }

            DA.SetDataList("Received", lastReceivedMessage);


        }

        #endregion Methods

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resources.CLI;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("F6005A32-8BB1-4258-B075-CBF7241E52C0"); }
        }
    }
}