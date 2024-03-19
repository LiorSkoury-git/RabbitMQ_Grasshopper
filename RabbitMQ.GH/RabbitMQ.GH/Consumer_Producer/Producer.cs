using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using RabbitMQ.Client.Events;
using Rhino.Geometry;
using RabbitMQ.Core;
using RabbitMQ.Client;
using System.Text;
using Rhino;
using RabbitMQ.GH.Properties;

namespace RabbitMQ.GH.Consumer_Producer
{
    /// <summary>
    /// Represents a RabbitMQ Producer inheriting from the GH_component class.
    /// </summary>
    public class Producer : GH_Component
    {
        #region Fields

        // <summary>
        /// Represents a TCP connection to a RabbitMQ broker.
        /// </summary>
        private Connection connection;

        #endregion Fields

        #region constructor

        /// <summary>
        /// Default constructor. Invokes the base class constructor.
        /// </summary>
        public Producer()
          : base("Producer", "Producer",
              "A producer rabbitMQ object",
              "RabbitMQ", "General")
        {
        }

        #endregion constructor

        /// <summary>
        /// Represents the exposure level of the component.
        /// </summary>
        /// <value>
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
        /// <param name="routingKey">Routing key assigned to the message.</param>
        /// <param name="message">Message to send.</param>
        private void sendMessage(string queue, string exchange, int exType, string routingKey, string message)
        {
            // If no queue name is provided, defaults to "".
            if (queue != "") { connection.setQueue(queue, Exclusive: false); }
            else { connection.setQueue(queue, Exclusive: true); }

            // If no exchange name is provided, defaults to "".
            if (exchange != "" && exchange != null) { connection.setExchange(exchange, exType); }
            else { exchange = ""; }

            // If no routing keys were provided, defaults the message to queue.
            if (routingKey == null) { routingKey = queue; }

            // Encodes the message.
            var encodeMessage = Encoding.UTF8.GetBytes(message);

            // Sends the message and confirms to the user.
            connection.channel.BasicPublish(exchange: exchange, routingKey: routingKey, null, encodeMessage);
            Rhino.RhinoApp.WriteLine("Message was sent");
        }
        
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Start or stop consuming", GH_ParamAccess.item);
            pManager.AddTextParameter("Host", "H", "Host url name", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Port", "P", "Port number of the broker", GH_ParamAccess.item);
            pManager.AddTextParameter("Body", "B", "Message to send to the consumer", GH_ParamAccess.list);
            pManager.AddTextParameter("Queue", "Q", "Queue name for the consumer", GH_ParamAccess.item);
            pManager.AddTextParameter("Exchange", "Ex", "Exchange name for the consumer", GH_ParamAccess.item);
            pManager.AddIntegerParameter("ExchangeType", "Ex_T", "Exchange type: 0=Fanout,1=Direct,2=Topic", GH_ParamAccess.item);
            pManager.AddTextParameter("RoutingKeys", "RK", "Routing keys as list", GH_ParamAccess.list);
            pManager.AddTextParameter("vHost", "VH", "Virtual host if needed", GH_ParamAccess.item);

            pManager[4].Optional = true;
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
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {   
            // Represents the state of the component. true if it is activated, false otherwise.
            bool b = false;
            // RabbitMQ state variables.
            string host = null;
            int port = 5672;
            string queue = "";
            string exchange = null;
            int exType = 1;
            List<string> routingKeys = new List<string>();
            List<string> messages = new List<string>();
            string vHost = null;


            // Get data from the component´s inputs
            DA.GetData("Run", ref b);
            DA.GetData("Host", ref host);
            DA.GetData("Port", ref port);
            DA.GetData("Queue", ref queue);
            DA.GetData("Exchange", ref exchange);
            DA.GetData("ExchangeType", ref exType);
            DA.GetDataList("RoutingKeys", routingKeys);
            DA.GetDataList("Body", messages);
            DA.GetData("vHost", ref vHost);

            if (b)
            {   
                setConnection(host, port, vHost);
                for (int i = 0; i < messages.Count; i++)
                {   
                    // Sets the routing key for the message.
                    string routingKey = null;
                    if (routingKeys.Count > 0)
                    {   
                        // Sets the routing key to the default value.
                        routingKey = routingKeys[routingKeys.Count - 1];
                        // If there is another element in routingKeys besides the default value,
                        // sets the routing key to the value corresponding to the message.
                        if (i < routingKeys.Count - 1)
                        {
                            routingKey = routingKeys[i];
                        }
                    }

                    sendMessage(queue, exchange, exType, routingKey, messages[i]);
                }
            }

            // Closes connection and handles errors.
            if (connection != null)
            {
                try
                {
                    connection.closeConnection();
                }
                catch (Exception ex)
                {
                    RhinoApp.WriteLine($"Exception in HandleMessageReceived: {ex.Message}");
                }
            }
        }

        #endregion Methods

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {

                return Resources.PRO2; 
                // You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("16A42FBF-F5CD-4F0E-B154-8F6FB31D5B50"); }
        }
    }
}