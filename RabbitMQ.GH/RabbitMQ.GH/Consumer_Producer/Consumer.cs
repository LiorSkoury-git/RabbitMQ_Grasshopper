using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Core;
using Rhino;
using System.Text;
using RabbitMQ.GH.Properties;

namespace RabbitMQ.GH.Consumer_Producer
{
    /// <summary>
    /// Represents a RabbitMQ consumer inheriting from the GH_component class.
    /// </summary>
    public class Consumer : GH_Component
    {
        #region Fields

        /// <summary>
        /// Represents the state of the timer that tracks message consumption time.
        /// </summary>
        private bool timerStarted;

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
        /// Represents a received message counter.
        /// </summary>
        private int counter;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Default constructor. Invokes the base class constructor and then extends it by adding the
        /// counter, lastReceivedMessage, and timerStarted members.
        /// </summary>
        public Consumer(): base("Consumer", "Consumer", "A consumer rabbitMQ object", "RabbitMQ", "General")
        {
            counter = 0;
            lastReceivedMessage = new List<object>() { "Turn on the Run to start consuming" };
            timerStarted = false;
        }

        #endregion Constructor

        /// <summary>
        /// Represents the exposure level of the component.
        /// </summary>
        /// /// <value>
        /// The value of 4 sets the Consumer component to be displayed in the second section of the Grasshopper toolbar.
        /// </value>
        public override GH_Exposure Exposure => (GH_Exposure)4;

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
            if (vHost==null) { vHost = "/"; }

            connection = new Connection();

            // Sets up the connection with authentication.
            if (RhinoDoc.ActiveDoc.RuntimeData.ContainsKey("RabbitMQ_USER") && RhinoDoc.ActiveDoc.RuntimeData.ContainsKey("RabbitMQ_PASS"))
            {
                connection.setFactory(host, port, (string)RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_USER"], (string)RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_PASS"],vHost);
            }
            // Sets up the connection without authentication.
            else
            {
                connection.setFactory(host, port,vHost);
            }

            // Handles connection errors.
            bool connect = connection.CreateConnection();
            if (!connect) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error connecting to RabbitMQ"); }
        }

        /// <summary>
        /// Sets the queue-exchange bindings for the connection.
        /// </summary>
        /// <param name="queue">Queue name.</param>
        /// <param name="exchange">Exchange name.</param>
        /// <param name="exType">Exchange type.</param>
        /// <param name="routingKeys">List of routing keys.</param>
        private void setConnectionDetails(string queue, string exchange, int exType, List<string> routingKeys)
        {

            // Sets up the message queue.
            if (queue != "") { connection.setQueue(queue, Exclusive: false); }
            else { connection.setQueue(queue, Exclusive: true); }

            // Sets up the message exchange.
            if (exchange != "" && exchange != null)
            {   
                // Sets up bindings if routing keys are provided.
                connection.setExchange(exchange, exType);
                if (routingKeys != null)
                {
                    if (routingKeys.Count > 0)
                    {
                        if (queue == "") { connection.setBinding(routingKeys); }
                        else { connection.setBinding(queue, exchange, routingKeys); }
                    }
                    else
                    {
                        routingKeys = new List<string>() { "" };
                        connection.setBinding(queue, exchange, routingKeys);
                    }
                }
                // Sets up bindings if routing keys are not provided.
                else
                {
                    routingKeys = new List<string>() { "" };
                    connection.setBinding(queue, exchange, routingKeys);
                }
            }


            // Creates a basic RabbitMQ consumer and subscribes the HandleReceivedMessage method to its Received event.
            var consumer = new EventingBasicConsumer(connection.channel);
            consumer.Received += HandleReceivedMessage;

            // Updates lastReceivedMessages.
            lastReceivedMessage = new List<object>() { "Waiting for messages..." };
            connection.channel.BasicConsume(queue: connection.queues[0].QueueName, autoAck: false, consumer: consumer);
            timerStarted = true;
        }

        /// <summary>
        /// Handles a newly received message.
        /// </summary>
        /// <param name="sender">Sender of the message.</param>
        /// <param name="ea">Deliver event arguments.</param>
        private void HandleMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            // Acknowledges the message.
            connection.channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            // Gets the message body decoded as String.
            byte[] body = ea.Body.ToArray();
            var data = Encoding.UTF8.GetString(body);
            // Handles the message's data.
            var result = handler.HandleMessage(data);
            // Updates lastReceivedMessage.
            lastReceivedMessage = result;
            ExpireSolution(true);
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
                counter++;
                RhinoApp.WriteLine("Message handled successfully.");
                ExpireSolution(true);
            }
            catch (Exception ex)
            {   
                //Error handling.
                RhinoApp.WriteLine($"Exception in HandleMessageReceived: {ex.Message}");
                ExpireSolution(true);
            }

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Start or stop consuming", GH_ParamAccess.item);
            pManager.AddTextParameter("Host", "H", "Host url name", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Port", "P", "Port number of the broker", GH_ParamAccess.item);
            pManager.AddTextParameter("Queue", "Q", "Queue name for the consumer", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Competing", "C", "True if competing consumers (default=fasle)", GH_ParamAccess.item);
            pManager.AddTextParameter("Exchange", "Ex", "Exchange name for the consumer", GH_ParamAccess.item);
            pManager.AddIntegerParameter("ExchangeType", "Ex_T", "Exchange type: 0=Fanout,1=Direct,2=Topic", GH_ParamAccess.item);
            pManager.AddTextParameter("RoutingKeys", "RK", "Routing keys as list", GH_ParamAccess.list);
            pManager.AddGenericParameter("Handler", "Ha", "Handler object to do somthing with the income data", GH_ParamAccess.item);
            pManager.AddTextParameter("vHost", "VH", "Virtual host if needed", GH_ParamAccess.item);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
            pManager[9].Optional = true;

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
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Represents the state of the component. true if it is activated false otherwise.
            bool b = false;
            // RabbitMQ state variables.
            string host = null;
            int port = 5672;
            string queue = "";
            bool competing = false;
            string exchange = null;
            int exType = 1;
            List<string> routingKeys = new List<string>();
            handler = new Handler();
            string vHost = null;

            // Get data from the component´s inputs
            DA.GetData("Run", ref b);
            DA.GetData("Host", ref host);
            DA.GetData("Port", ref port);
            DA.GetData("Queue", ref queue);
            DA.GetData("Competing", ref competing);
            DA.GetData("Exchange", ref exchange);
            DA.GetData("ExchangeType", ref exType);
            DA.GetDataList("RoutingKeys", routingKeys);
            DA.GetData("Handler", ref handler);
            DA.GetData("vHost",ref vHost);


            if (b)
            {
                if (!timerStarted)
                {
                    setConnection(host, port, vHost);
                    setConnectionDetails(queue, exchange, exType, routingKeys);
                }
            }
            else
            {
                if (timerStarted)
                {
                    connection.closeConnection(); // Close the connection setup if not done yet
                    lastReceivedMessage = new List<object>() { "Turn on the Run to start consuming" };
                    timerStarted = false;
                }
            }

            DA.SetDataList("Received", lastReceivedMessage);

        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get 
            {
                return Resources.CONS;
            }
            //return Resources.

        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("656192ec-5cc8-43ee-8b8d-09970a8597c9");

        #endregion Methods
    }
}