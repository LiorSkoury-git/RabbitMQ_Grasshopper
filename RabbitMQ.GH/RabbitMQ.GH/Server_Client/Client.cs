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
    public class Client : GH_Component
    {

        private bool timerStarted;
        private bool messageSent;
        private string currentMessage;
        private List<object> lastReceivedMessage;
        private Connection connection;
        private Handler handler;
        private EventingBasicConsumer replyConsumer;
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
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

        public override GH_Exposure Exposure => (GH_Exposure)2;

        private void setConnection(string host, int port, string vHost)
        {
            if (vHost == null) { vHost = "/"; }
            connection = new Connection();
            if (RhinoDoc.ActiveDoc.RuntimeData.ContainsKey("RabbitMQ_USER") && RhinoDoc.ActiveDoc.RuntimeData.ContainsKey("RabbitMQ_PASS"))
            {
                connection.setFactory(host, port, (string)RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_USER"], (string)RhinoDoc.ActiveDoc.RuntimeData["RabbitMQ_PASS"], vHost);
            }
            else
            {
                connection.setFactory(host, port, vHost);
            }
            bool connect = connection.CreateConnection();
            if (!connect) { AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Error connecting to RabbitMQ"); }
        }

        private void sendMessage(string queue, string exchange, int exType, string message)
        {

            if (queue != "") { connection.setQueue(queue, Exclusive: false); }
            else { connection.setQueue(queue, Exclusive: true); }

            if (exchange != "" && exchange != null)
            {
                connection.setExchange(exchange, exType);
            }
            else { exchange = ""; }

            var replyQueue = connection.channel.QueueDeclare("", exclusive: true);


            replyConsumer = new EventingBasicConsumer(connection.channel);
            replyConsumer.Received += HandleReceivedMessage;
            connection.channel.BasicConsume(queue: replyQueue.QueueName, autoAck: false, consumer: replyConsumer);

            var properties = connection.channel.CreateBasicProperties();
            properties.ReplyTo = replyQueue.QueueName;
            properties.CorrelationId = Guid.NewGuid().ToString();

            var encodeMessage = Encoding.UTF8.GetBytes(message);
            connection.channel.BasicPublish(exchange, routingKey: queue, properties, encodeMessage);
            messageSent = true;
            timerStarted = true;
        }

        private void HandleReceivedMessage(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                RhinoApp.WriteLine("Handling message...");
                connection.channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                var result = handler.HandleMessage(body);
                lastReceivedMessage = result;
                RhinoApp.WriteLine("Message handled successfully.");
                ExpireSolution(true);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Exception in HandleMessageReceived: {ex.Message}");
                //ExpireSolution(true);
            }
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
            bool b = false;
            string host = null;
            int port = 5672;
            string queue = "";
            string exchange = null;
            int exType = 1;
            string message = null;
            handler = new Handler();
            string vHost = null;

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
                    lastReceivedMessage = new List<object>() { "Turn on the Run to start consuming" };
                    timerStarted = false;
                    messageSent = false;
                    currentMessage = null;
                }
            }

            DA.SetDataList("Received", lastReceivedMessage);


        }

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